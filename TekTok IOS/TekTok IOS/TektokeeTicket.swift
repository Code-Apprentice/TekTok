//
//  TektokeeTicket.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 12/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit

class TektokeeTicket: UIViewController {
    
    var checkBool = true
    override func viewDidLoad() {
        super.viewDidLoad()
        Tektokkername.text = "Your TekTokker: " + GVar.tektokker
        print("load")
        
        // Do any additional setup after loading the view.
    }
    var taskId:UIBackgroundTaskIdentifier = -1
    
    func beginBackgroundUpdateTask() {
        taskId = UIApplication.sharedApplication().beginBackgroundTaskWithExpirationHandler({})
    }
    
    func endBackgroundUpdateTask(taskID: UIBackgroundTaskIdentifier) {
        UIApplication.sharedApplication().endBackgroundTask(taskID)
    }
    
    
    func update(sender: AnyObject ,taskIDe: UIBackgroundTaskIdentifier){
        checkBool = false
        
        
        let settings = UIApplication.sharedApplication().currentUserNotificationSettings()
        
        if settings!.types == .None {
            let ac = UIAlertController(title: "Please allow notifications!", message: "No permission to schedule Notification", preferredStyle: .Alert)
            ac.addAction(UIAlertAction(title: "OK", style: .Default, handler: nil))
            dispatch_async(dispatch_get_main_queue()) {
                self.presentViewController(ac, animated: true, completion: nil)
            }
        }
        else{
            print("notif")
            let notification = UILocalNotification()
            notification.alertBody = "Ticket Timed out!"
            notification.alertAction = "open" // text that is displayed after "slide to..." on the lock screen - defaults to "slide to view"
            notification.fireDate = NSDate(timeIntervalSinceNow: 5)
            notification.soundName = UILocalNotificationDefaultSoundName // play default sound
            
            UIApplication.sharedApplication().scheduleLocalNotification(notification)
            
        }
        let con = MySQL.Connection()
        let db_name = "sql6133445"

        do{
            try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
            try con.use(db_name)
            try con.query("UPDATE Tickets SET TimedOut = 1 WHERE TeacherName = '"+GVar.fullName+"'")
        }catch(let e){
            print(e)
        }

        
        
        
        dispatch_async(dispatch_get_main_queue()) {
            let myAlert = UIAlertController(title: "Ticket Timed Out", message: "Your Ticket has timed out", preferredStyle: UIAlertControllerStyle.Alert);
            let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                
                                               //myAlert.dismissViewControllerAnimated(false, completion: nil)
                do{
                    try con.query("DELETE FROM Tickets WHERE TeacherName = '" + GVar.fullName + "'")
                }catch(let e){
                    print(e)
                }
                
                self.performSegueWithIdentifier("backtoTekTokee", sender: self);
                
                self.endBackgroundUpdateTask(self.taskId)
            }
            myAlert.addAction(okAction);
            self.presentViewController(myAlert, animated: true, completion: nil)
        }
        self.endBackgroundUpdateTask(taskId)
        
        self.performSegueWithIdentifier("backtoTekTokee", sender: nil);
    }
    
    
    override func viewDidAppear(animated: Bool) {
        
        
        self.navigationItem.hidesBackButton = true
        let newBackButton = UIBarButtonItem(title: "Back", style: UIBarButtonItemStyle.Plain, target: self, action: #selector(TicketViewController.back(_:)))
        self.navigationItem.leftBarButtonItem = newBackButton;
        
        
        dispatch_async( dispatch_get_global_queue(QOS_CLASS_BACKGROUND, 0), {
            
            
            
            NSTimer.scheduledTimerWithTimeInterval(900, target: self, selector: #selector(TektokeeViewController.update(_:)), userInfo: nil, repeats: true)
            
            let con = MySQL.Connection()
            let db_name = "sql6133445"
            do{
                try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
                try con.use(db_name)
            }catch(let e){
                print(e)
            }
            
            
            while(self.checkBool){
                
                print("Check Fin/Can")
                do{
                    
                    let select_stmt = try con.query("SELECT Cancelled, Finished FROM Tickets WHERE TeacherName = '"+GVar.fullName+"' ORDER BY Accepted DESC LIMIT 1")
                    
                    let rows = try select_stmt.readAllRows()
                    if(!(rows!.isEmpty)){
                        if(String(rows![0][0]["Cancelled"]) == "Optional(1)"){
                            dispatch_async(dispatch_get_main_queue()) {
                                let myAlert = UIAlertController(title: "Ticket Cancelled", message: "Ticket has been cancelled by TekTokker", preferredStyle: UIAlertControllerStyle.Alert);
                                let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                                    
                                    //myAlert.dismissViewControllerAnimated(false, completion: nil)
                                    do{
                                        try con.query("DELETE FROM Tickets WHERE TeacherName = '" + GVar.fullName + "'")
                                    }catch(let e){
                                        print(e)
                                    }
                                    
                                    self.performSegueWithIdentifier("backtoTekTokee", sender: self);
                                    
                                    self.endBackgroundUpdateTask(self.taskId)
                                }
                                myAlert.addAction(okAction);
                                self.presentViewController(myAlert, animated: true, completion: nil)
                            }
                            
                            break
                        }
                        if(String(rows![0][0]["Finished"]) == "Optional(1)"){
                            dispatch_async(dispatch_get_main_queue()) {
                                let myAlert = UIAlertController(title: "Task Finished", message: "Task has been completed by TekTokker", preferredStyle: UIAlertControllerStyle.Alert);
                                let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                                    //myAlert.dismissViewControllerAnimated(false, completion: nil)
                                    do{
                                        try con.query("DELETE FROM Tickets WHERE TeacherName = '" + GVar.fullName + "'")
                                    }catch(let e){
                                        print(e)
                                    }
                                    self.performSegueWithIdentifier("backtoTekTokee", sender: nil);
                                    self.endBackgroundUpdateTask(self.taskId)
                                    
                                }
                                myAlert.addAction(okAction);
                                self.presentViewController(myAlert, animated: true, completion: nil)
                                
                                
                            }
                            break
                        }
                        
                        
                    }
                    else{
                        break
                    }
                    
                    
                }
                catch(let e){
                    print(e)
                    break
                }
                
            }
            
            self.endBackgroundUpdateTask(self.taskId)
            
            
        })
        
        print("here")
        
        
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    func back(sender: UIBarButtonItem) {
        print("back")
        // Perform your custom actions
        // ...
        // Go back to the previous ViewController
        dispatch_async(dispatch_get_main_queue()) {
            let myAlert = UIAlertController(title: "Cancel Request?", message: "Do you want to cancel your request?", preferredStyle: UIAlertControllerStyle.Alert);
            let okAction = UIAlertAction(title: "Yes", style: UIAlertActionStyle.Default){(ACTION) in
                
                let con = MySQL.Connection()
                let db_name = "sql6133445"
                do{
                    
                    try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
                    
                    try con.use(db_name)
                    
                    try con.query("DELETE FROM Tickets WHERE TeacherName = '" + GVar.fullName + "'")
                    
                    GVar.requested = false
                    try con.close()
                    
                    
                    
                }catch (let e){
                    print(e)
                }
                self.endBackgroundUpdateTask(self.taskId)
                self.performSegueWithIdentifier("backtoTekTokee", sender: nil);
                
            }
            let noAction = UIAlertAction(title: "No", style: UIAlertActionStyle.Default){(ACTION) in
            }
            myAlert.addAction(okAction)
            myAlert.addAction(noAction)
            self.presentViewController(myAlert, animated: true, completion: nil)
        }
    }
    
    @IBOutlet weak var Tektokkername: UILabel!
    
    @IBAction func FinishedTicket(sender: AnyObject) {
        print("fin")
        dispatch_async(dispatch_get_main_queue()) {
            let myAlert = UIAlertController(title: "Cancel Request?", message: "Do you want to cancel your request?", preferredStyle: UIAlertControllerStyle.Alert);
            let okAction = UIAlertAction(title: "Yes", style: UIAlertActionStyle.Default){(ACTION) in
                
                let con = MySQL.Connection()
                let db_name = "sql6133445"
                do{
                    
                    try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
                    
                    try con.use(db_name)
                    
                    try con.query("DELETE FROM Tickets WHERE TeacherName = '" + GVar.fullName + "'")
                    
                    GVar.requested = false
                    try con.close()
                    
                    self.endBackgroundUpdateTask(self.taskId)
                    
                }catch (let e){
                    print(e)
                }
                
                
                
                self.performSegueWithIdentifier("backtoTekTokee", sender: nil);
                
            }
            let noAction = UIAlertAction(title: "No", style: UIAlertActionStyle.Default){(ACTION) in
            }
            myAlert.addAction(okAction)
            myAlert.addAction(noAction)
            self.presentViewController(myAlert, animated: true, completion: nil)
        }
    }
    
    /*
     // MARK: - Navigation
     
     // In a storyboard-based application, you will often want to do a little preparation before navigation
     override func prepareForSegue(segue: UIStoryboardSegue, sender: AnyObject?) {
     // Get the new view controller using segue.destinationViewController.
     // Pass the selected object to the new view controller.
     }
     */
    
}
