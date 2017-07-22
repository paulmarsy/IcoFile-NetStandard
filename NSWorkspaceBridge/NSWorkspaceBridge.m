//
//  NSWorkspaceBridge.m
//  NSWorkspaceBridge
//
//  Created by Paul Marston on 09/06/2017.
//  Copyright © 2017 Paul Marston. All rights reserved.
//

#import "NSWorkspaceBridge.h"
#import <AppKit/NSWorkspace.h>
#import <AppKit/NSImage.h>

bool NSWorkspaceSetIconForFile(char* setIcon, char* forFile){
    NSImage* image = [[NSImage alloc] initWithContentsOfFile: [NSString stringWithUTF8String:setIcon]];
    bool success = [[NSWorkspace sharedWorkspace] setIcon:image
                                                  forFile: [NSString stringWithUTF8String:forFile]
                                                  options:0];
    return success;
}
void Test(char* msg){
    NSString* msgString = [NSString stringWithUTF8String:msg];
    NSLog(@"%@", msgString);
}
