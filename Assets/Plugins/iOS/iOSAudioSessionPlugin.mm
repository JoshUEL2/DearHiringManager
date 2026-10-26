#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

@interface iOSAudioBridge : NSObject
+ (void)configureAudioSession;
@end

@implementation iOSAudioBridge

+ (void)configureAudioSession {
    dispatch_async(dispatch_get_main_queue(), ^{
        NSError *error = nil;
        AVAudioSession *session = [AVAudioSession sharedInstance];
        // AVAudioSessionCategoryPlayback ensures audio plays even if the physical silent switch is engaged
        BOOL success = [session setCategory:AVAudioSessionCategoryPlayback
                                withOptions:(AVAudioSessionCategoryOptionMixWithOthers)
                                      error:&error];
        if (!success || error) {
            NSLog(@"[iOSAudioBridge] Failed to set AVAudioSessionCategoryPlayback with options: %@", error.localizedDescription);
            [session setCategory:AVAudioSessionCategoryPlayback error:nil];
        }
        [session setActive:YES error:nil];
        NSLog(@"[iOSAudioBridge] AVAudioSession configured successfully for Playback.");
    });
}

@end

extern "C" {
    void _iOS_ConfigureAudioSession() {
        [iOSAudioBridge configureAudioSession];
    }
}
