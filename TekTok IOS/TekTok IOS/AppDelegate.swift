//
//  AppDelegate.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 09/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit
import Google
import GoogleSignIn
import MySqlSwiftNative
import Foundation


//global variables
struct GVar{
    static var userId = ""
    static var idToken = ""
    static var fullName = ""
    static var givenName = ""
    static var familyName = ""
    static var email = ""
    static var requested = false
    static var keepChecking = true
    static var tektokker = ""
    static var tickets:[Ticket] = []
}

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate, GIDSignInDelegate {

    
    var window: UIWindow?

    //google sign in stuff

    func application(application: UIApplication,
                     didFinishLaunchingWithOptions launchOptions: [NSObject: AnyObject]?) -> Bool {
        // Initialize sign-in
        var configureError: NSError?
        GGLContext.sharedInstance().configureWithError(&configureError)
        assert(configureError == nil, "Error configuring Google services: \(configureError)")
        
        GIDSignIn.sharedInstance().delegate = self
        
        return true
    }
    
    func application(application: UIApplication,
                     openURL url: NSURL, options: [String: AnyObject]) -> Bool {
        return GIDSignIn.sharedInstance().handleURL(url,
                                                    sourceApplication: options[UIApplicationOpenURLOptionsSourceApplicationKey] as? String,
                                                    annotation: options[UIApplicationOpenURLOptionsAnnotationKey])
    }
    
    func signIn(signIn: GIDSignIn!, didSignInForUser user: GIDGoogleUser!,
                withError error: NSError!) {
        if (error == nil) {
            //get user information and add it to the global variables list
            
            GVar.userId = user.userID
            GVar.idToken = user.authentication.idToken
            GVar.fullName = user.profile.name
            GVar.givenName = user.profile.givenName
            GVar.familyName = user.profile.familyName
            GVar.email = user.profile.email
           
            let notificationSettings = UIUserNotificationSettings(forTypes: [.Alert, .Badge, .Sound], categories: nil)
            UIApplication.sharedApplication().registerUserNotificationSettings(notificationSettings)
            
            if GVar.email.rangeOfString("@alice-smith.edu.my") != nil{
                //open connection
                let con = MySQL.Connection()
                //database name
                let db_name = "sql6133445"
                do{
                    // open a new connection
                    try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
                    
                    try con.use(db_name)
                    
                    // create a table for our tests
                    
                    
                    // prepare a new statement for insert
                    //let ins_stmt = try con.prepare("INSERT INTO test(age, cash, name) VALUES(?,?,?)")
                    
                    // prepare a new statement for select
                    //find users
                    let select_stmt = try con.query("SELECT TekTokker FROM Users WHERE Email = '"+GVar.email+"'")
                    let rows = try select_stmt.readAllRows()
                    
                    let select_stmttest = try con.query("SELECT * FROM Users")
                    let rowstest = try select_stmttest.readAllRows()
                    print(rowstest)
                    if(rows!.isEmpty){//New User
                        //create new user
                        let ins_stmt = try con.prepare("INSERT INTO Users(Email, Name, TekTokker, Hired) VALUES(?,?,?,?)")
                        try ins_stmt.exec([GVar.email, GVar.fullName, "0", "0"])
                        let storyboard = UIStoryboard(name: "Main", bundle: nil)
                        let viewController: TektokeeViewController = storyboard.instantiateViewControllerWithIdentifier("TektokeeViewController") as! TektokeeViewController
                        
                        // Then push that view controller onto the navigation stack
                        let rootViewController = self.window!.rootViewController as! UINavigationController
                        rootViewController.pushViewController(viewController, animated: true)
                        
                    }
                    else{//returning user
                        
                        if(String(rows![0][0]["TekTokker"]) == "Optional(0)"){ //not a tektokker
                            let storyboard = UIStoryboard(name: "Main", bundle: nil)
                            let viewController: TektokeeViewController = storyboard.instantiateViewControllerWithIdentifier("TektokeeViewController") as! TektokeeViewController
                            
                            // Then push that view controller onto the navigation stack
                            let rootViewController = self.window!.rootViewController as! UINavigationController
                            rootViewController.pushViewController(viewController, animated: true)
                            
                        }
                        else{ //tektokker
                            let storyboard = UIStoryboard(name: "Main", bundle: nil)
                            let viewController: TektokkerViewController = storyboard.instantiateViewControllerWithIdentifier("TektokkerViewController") as! TektokkerViewController
                            
                            // Then push that view controller onto the navigation stack
                            let rootViewController = self.window!.rootViewController as! UINavigationController
                            
                            rootViewController.pushViewController(viewController, animated: true)
                            print("here")
                            
                        }
                    }
                    
                    
                    try con.close()
                }
                catch (let e) {
                    print(e)
                }
                

            }
            else{
                //alert object creation and presentation
                let myAlert = UIAlertController(title: "No Access", message: "You do not have an Alice Smith School Email", preferredStyle: UIAlertControllerStyle.Alert);
                let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                    
                }
                myAlert.addAction(okAction);
                self.window?.rootViewController?.presentViewController(myAlert, animated: true, completion: nil)
                GIDSignIn.sharedInstance().signOut()
                GIDSignIn.sharedInstance().disconnect()
            }
            
            
        } else {
            print("\(error.localizedDescription)")
        }
    }
    
    func signIn(signIn: GIDSignIn!, didDisconnectWithUser user:GIDGoogleUser!,
                withError error: NSError!) {
        // Perform any operations when the user disconnects from app here.
        // ...
    }


    func applicationWillResignActive(application: UIApplication) {
        // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
        // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
    }

    func applicationDidEnterBackground(application: UIApplication) {
        // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
        // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
    }

    func applicationWillEnterForeground(application: UIApplication) {
        // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
    }

    func applicationDidBecomeActive(application: UIApplication) {
        // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
    }

    func applicationWillTerminate(application: UIApplication) {
        // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
    }


}

