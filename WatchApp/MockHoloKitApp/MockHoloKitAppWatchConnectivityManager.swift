import WatchConnectivity

enum Reality: Int {
    case nothing = 0
    case mofaTheTraining = 1
}

class MockHoloKitAppWatchConnectivityManager: NSObject, ObservableObject {
    
    @Published var currentReality: Reality = .nothing
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
        
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
        }
    }
    
    func HasPairedAppleWatch() -> Bool {
        return self.wcSession.isPaired;
    }
    
    func IsWatchAppInstalled() -> Bool {
        return self.wcSession.isWatchAppInstalled;
    }
    
    func Activate() {
        self.wcSession.activate()
    }
    
    func IsReachable() -> Bool {
        return self.wcSession.isReachable
    }
    
    func UpdateCurrentReality(_ realityIndex: Int) {
        let context = ["CurrentReality" : realityIndex];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Updated current reality")
        } catch {
            print("Failed to update current reality")
        }
    }
}

extension MockHoloKitAppWatchConnectivityManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("WCSession activated");
        } else {
            print("WCSession activation failed")
        }
    }
    
    func sessionDidBecomeInactive(_ session: WCSession) {
        
    }
    
    func sessionDidDeactivate(_ session: WCSession) {
        
    }
}

