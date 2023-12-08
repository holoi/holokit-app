// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <WatchConnectivity/WatchConnectivity.h>

// This must be identical to the enum on the Watch side.
typedef enum {
    None = 0,
    MOFA = 1
} HoloKitWatchPanel;

@interface HoloKitAppWatchConnectivityManager: NSObject

@property (nonatomic, strong) WCSession *wcSession;

@property (assign) HoloKitWatchPanel watchPanel;

+ (id)sharedInstance;

@end
