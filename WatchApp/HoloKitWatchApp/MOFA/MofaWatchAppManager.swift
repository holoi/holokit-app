import Foundation
import WatchKit
import WatchConnectivity
import CoreMotion
import HealthKit
import simd

enum MofaView: Int {
    case readyView = 0
    case handednessView = 1
    case fightingView = 2
    case resultView = 3
}

enum MofaRoundResult: Int {
    case victory = 0
    case defeat = 1
    case draw = 2
}

enum MofaMagicSchool: Int {
    case mysticArt = 0
    case thunder = 1
    case harryPotter = 2
}

enum MofaWatchState: Int {
    case normal = 0
    case ground = 1
}

class MofaWatchAppManager: NSObject, ObservableObject {
    
    // Keep a reference to the watch app main manager
    var holokitWatchAppManager: HoloKitWatchAppManager?
    
    @Published var currentView: MofaView = .readyView
    
    @Published var magicSchool: MofaMagicSchool = .mysticArt
    
    @Published var isRightHanded: Bool = true {
        didSet {
            UserDefaults.standard.set(isRightHanded, forKey: "UserHandedness")
        }
    }
    
//    @Published var isFighting: Bool = false
    
    // Round result stats
    @Published var roundResult: MofaRoundResult = .victory
    @Published var kill: Int = 0
    @Published var hitRate: Int = 0
    
    var wcSession: WCSession!
    
    let motionManager = CMMotionManager()
    
    let healthStore = HKHealthStore()
    var workoutSession: HKWorkoutSession?
    var builder: HKLiveWorkoutBuilder?
    
    let deviceMotionUpdateInterval: Double = 0.016
    var currentState: MofaWatchState = .normal
    let sharedInputCd: Double = 0.5
    var lastInputTime: Double = 0
    // This vector varies with handedness and digital crown orientation
    var groundVector = simd_double3(-1, 0, 0)
    var lastStartRoundTime: Double = 0
    
    let meterToFeet: Double = 3.2808
    
    override init() {
        super.init()
        if (WCSession.isSupported()) {
            self.wcSession = WCSession.default
        }
        requestHealthKitAuthorization()
        if UserDefaults.standard.object(forKey: "UserHandedness") != nil  {
            self.isRightHanded = UserDefaults.standard.bool(forKey: "UserHandedness")
        }
    }
    
    func takeControlWatchConnectivitySession() {
        if (WCSession.isSupported()) {
            self.wcSession = WCSession.default
            self.wcSession.delegate = self
            self.wcSession.activate()
        }
    }
    
    func requestHealthKitAuthorization() {
        let typesToShare: Set = [ HKQuantityType.workoutType() ]
        
        let typesToRead: Set = [
            HKQuantityType.quantityType(forIdentifier: .heartRate)!,
            HKQuantityType.quantityType(forIdentifier: .activeEnergyBurned)!,
            HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning)!,
            HKObjectType.activitySummaryType()
        ]
        
        // Request authorization for those quantity types.
        healthStore.requestAuthorization(toShare: typesToShare, read: typesToRead) { (success, error) in
            if error != nil {
                print("Got error when requesting HealthKit authorization: \(String(describing: error))")
                return
            }
            if success {
                //print("HealthKit authorization requested")
            } else {
                print("Falied to request HealthKit authorization")
            }
        }
    }
    
    public func startWorkout() {
        resetWorkout()
        
        let configuration = HKWorkoutConfiguration()
        configuration.activityType = .play
        configuration.locationType = .indoor
        
        // Create the session and obtain the workout builder
        do {
            workoutSession = try HKWorkoutSession(healthStore: healthStore, configuration: configuration)
            builder = workoutSession?.associatedWorkoutBuilder()
        } catch {
            print("Failed to create workout session")
            return
        }
        
        workoutSession?.delegate = self
        builder?.delegate = self
        
        // Set the workout builder's data source
        builder?.dataSource = HKLiveWorkoutDataSource(healthStore: healthStore, workoutConfiguration: configuration)
        
        let startDate = Date()
        workoutSession?.startActivity(with: startDate)
        builder?.beginCollection(withStart: startDate) { (success, error) in
            // The workout has started
        }
    }
    
    public func endWorkout() {
        workoutSession?.end()
        workoutSession = nil
        sendHealthDataMessage()
        self.currentView = .resultView
    }
    
    public func startCoreMotion() {
        // Check if we can start core motion now
        if (motionManager.isDeviceMotionAvailable && !motionManager.isDeviceMotionActive) {
            motionManager.deviceMotionUpdateInterval = self.deviceMotionUpdateInterval
            // Check handedness
            if (self.isRightHanded) {
                if (WKInterfaceDevice.current().crownOrientation == .right) {
                    self.groundVector = simd_double3(-1, 0, 0)
                } else {
                    self.groundVector = simd_double3(1, 0, 0)
                }
            }
            else {
                if (WKInterfaceDevice.current().crownOrientation == .right) {
                    self.groundVector = simd_double3(1, 0, 0)
                } else {
                    self.groundVector = simd_double3(-1, 0, 0)
                }
            }
            motionManager.startDeviceMotionUpdates(using: .xMagneticNorthZVertical, to: OperationQueue.current!) { (data: CMDeviceMotion?, error: Error?) in
                if error != nil {
                    return
                }
                
                
                let currentTime = ProcessInfo.processInfo.systemUptime
                // If we should stop the round automatically now
//                if (currentTime - self.lastStartRoundTime > 120) {
//                    DispatchQueue.main.async {
//                        self.stopRound()
//                    }
//                }
                
                guard let acceleration: CMAcceleration = data?.userAcceleration else {
                    return
                }
                guard let gravity = data?.gravity else {
                    return
                }
                let gravityVector3 = simd_double3(gravity.x, gravity.y, gravity.z)
                
                if (currentTime - self.lastInputTime > self.sharedInputCd) {
                    if (abs(acceleration.x) > 1.6) {
                        print("Triggered")
                        self.sendWatchTriggeredMessage()
                        self.lastInputTime = currentTime
                        return
                    }
                }
                
                if (simd_dot(gravityVector3, self.groundVector) > 0.7) {
                    if (self.currentState != .ground) {
                        print("Changed to ground")
                        self.currentState = .ground
                        self.sendWatchStateChangedMessage(watchState: self.currentState)
                        return
                    }
                } else {
                    if (self.currentState != .normal) {
                        print("Changed to normal")
                        self.currentState = .normal
                        self.sendWatchStateChangedMessage(watchState: self.currentState)
                        return
                    }
                }
            }
        }
    }
    
    public func endCoreMotion() {
        if (motionManager.isDeviceMotionAvailable && motionManager.isDeviceMotionActive) {
            motionManager.stopDeviceMotionUpdates()
        }
    }
    
    public func startRound() {
        self.lastStartRoundTime = ProcessInfo.processInfo.systemUptime
        startWorkout()
        startCoreMotion()
        self.currentView = .fightingView
    }
    
    public func stopRound() {
        endCoreMotion()
        endWorkout()
    }
    
    func sendStartRoundMessage() {
        self.wcSession = WCSession.default
        let message = ["StartRound": 0];
        self.wcSession.sendMessage(message, replyHandler: nil)
        print("Start round message sent")
    }
    
    func sendWatchTriggeredMessage() {
        let message = ["WatchTriggered" : 0];
        self.wcSession.sendMessage(message, replyHandler: nil)
    }
    
    func sendWatchStateChangedMessage(watchState: MofaWatchState) {
        let message = ["WatchState" : watchState.rawValue]
        self.wcSession.sendMessage(message, replyHandler: nil)
    }
    
    func sendHealthDataMessage() {
        let dist = Float(self.distance)
        let calories = Float(self.activeEnergy)
        let message = [ "Distance" : dist, "Calories" : calories ]
        self.wcSession.sendMessage(message, replyHandler: nil)
    }
    
    // MARK: - Workout Metrics
    @Published var averageHeartRate: Double = 0
    @Published var heartRate: Double = 0
    @Published var activeEnergy: Double = 0
    @Published var distance: Double = 0
    @Published var workout: HKWorkout?
    
    func updateForStatistics(_ statistics: HKStatistics?) {
        guard let statistics = statistics else { return }
        
        DispatchQueue.main.async {
            switch statistics.quantityType {
            case HKQuantityType.quantityType(forIdentifier: .heartRate):
                let heartRateUnit = HKUnit.count().unitDivided(by: HKUnit.minute())
                self.heartRate = statistics.mostRecentQuantity()?.doubleValue(for: heartRateUnit) ?? 0
                self.averageHeartRate = statistics.averageQuantity()?.doubleValue(for: heartRateUnit) ?? 0
            case HKQuantityType.quantityType(forIdentifier: .activeEnergyBurned):
                let energyUnit = HKUnit.kilocalorie()
                self.activeEnergy = statistics.sumQuantity()?.doubleValue(for: energyUnit) ?? 0
            case HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning), HKQuantityType.quantityType(forIdentifier: .distanceCycling):
                let meterUnit = HKUnit.meter()
                self.distance = statistics.sumQuantity()?.doubleValue(for: meterUnit) ?? 0
            default:
                return
            }
        }
    }
    
    func resetWorkout() {
        builder = nil
        workout = nil
        workoutSession = nil
        activeEnergy = 0
        averageHeartRate = 0
        heartRate = 0
        distance = 0
    }
}

// MARK: - WCSessionDelegate
extension MofaWatchAppManager: WCSessionDelegate {
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("[MOFA] Apple Watch's WCSession activated");
        } else {
            print("[MOFA] Apple Watch's WCSession activation failed");
        }
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        if let roundStart = applicationContext["RoundStart"] as? Bool {
            if (roundStart == true) {
                print("MOFA round started")
                DispatchQueue.main.async {
                    if let magicSchoolIndex = applicationContext["MagicSchool"] as? Int {
                        if let magicSchool = MofaMagicSchool(rawValue: magicSchoolIndex) {
                            self.magicSchool = magicSchool
                        }
                    }
                    self.startRound()
                }
            }
            return
        }
        
        if let roundOver = applicationContext["RoundOver"] as? Bool {
            if (roundOver == true) {
                print("MOFA round ended")
                if let roundResultIndex = applicationContext["RoundResult"] as? Int {
                    if let roundResult = MofaRoundResult(rawValue: roundResultIndex) {
                        DispatchQueue.main.async {
                            self.roundResult = roundResult
                        }
                    }
                }
                if let kill = applicationContext["Kill"] as? Int {
                    DispatchQueue.main.async {
                        self.kill = kill
                    }
                }
                if let hitRate = applicationContext["HitRate"] as? Int {
                    DispatchQueue.main.async {
                        self.hitRate = Int(hitRate)
                    }
                }
                DispatchQueue.main.async {
                    self.stopRound()
                }
            }
            return
        }
        
        if let currentWatchPanel = applicationContext["CurrentWatchPanel"] as? Int {
            if (currentWatchPanel == 0) {
                DispatchQueue.main.async {
                    self.stopRound()
                }
                self.holokitWatchAppManager?.session(session, didReceiveApplicationContext: applicationContext)
            }
            return
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any], replyHandler: @escaping ([String : Any]) -> Void) {
        if message["QueryWatchState"] is Int {
            let replyMessage = ["WatchState" : self.currentState.rawValue];
            replyHandler(replyMessage)
        }
    }
}

// MARK: - HKWorkoutSessionDelegate
extension MofaWatchAppManager: HKWorkoutSessionDelegate {
    func workoutSession(_ workoutSession: HKWorkoutSession, didChangeTo toState: HKWorkoutSessionState, from fromState: HKWorkoutSessionState, date: Date) {
        //print("workoutSessionDidChangeTo \(toState) from \(fromState)")
        DispatchQueue.main.async {
            
        }
        
        // Wait for the session to transition states before ending the builder.
        if toState == .ended {
            builder?.endCollection(withEnd: date) { (success, error) in
                self.builder?.finishWorkout { (workout, error) in
                    DispatchQueue.main.async {
                        self.workout = workout
                    }
                }
            }
        }
    }
    
    func workoutSession(_ workoutSession: HKWorkoutSession, didFailWithError error: Error) {
        
    }
}

// MARK: - HKLiveWorkoutBuilderDelegate
extension MofaWatchAppManager: HKLiveWorkoutBuilderDelegate {
    func workoutBuilderDidCollectEvent(_ workoutBuilder: HKLiveWorkoutBuilder) {
        //print("event: \(String(describing: workoutBuilder.workoutEvents.last?.type))")
    }
    
    func workoutBuilder(_ workoutBuilder: HKLiveWorkoutBuilder, didCollectDataOf collectedTypes: Set<HKSampleType>) {
        //print("workoutBuilder didCollectDataOf")
        for type in collectedTypes {
            guard let quantityType = type as? HKQuantityType else {
                return
            }
            
            let statistics = workoutBuilder.statistics(for: quantityType)
            
            // Update the published values.
            updateForStatistics(statistics)
        }
    }
}

// MARK: - Math
extension MofaWatchAppManager {
    func rad2deg(_ number: Double) -> Double {
        return number * 180 / .pi
    }
    
    func sameSign(_ num1: Double, _ num2: Double) -> Bool {
        return num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0
    }
}
