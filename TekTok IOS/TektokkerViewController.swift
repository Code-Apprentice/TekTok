//
//  TektokkerViewController.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 09/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit


class Ticket: Equatable {
    var teachername:String = ""
    var room:String = ""
    var timestamp:String = ""
    var extraInfo:String = ""
    let equatable:Bool = true
}


func ==(lhs:Ticket, rhs:Ticket) -> Bool { // Implement Equatable
    return lhs.equatable == rhs.equatable
}

class TektokkerViewController: UIViewController {
    
    
    @IBOutlet weak var buttonOut: UIButton!
    
    @IBOutlet weak var logOUT: UIButton!
    @IBAction func logoutaction(sender: AnyObject) {
        GVar.keepChecking = false
        GIDSignIn.sharedInstance().signOut()
        dispatch_async(dispatch_get_main_queue()) {
            self.navigationController?.popToRootViewControllerAnimated(true)
        }
    }
    
    @IBOutlet weak var Extrainfooutlet: UILabel!
    @IBOutlet weak var TeacherNameOutlet: UILabel!
    @IBOutlet weak var readyOutlet: UILabel!
    @IBOutlet weak var welcomeMessage: UILabel!
    @IBOutlet weak var AwaitImage: UIImageView!
    let reqavailable = false
    
    var tickets:[Ticket] = []
    var emptytickets:[Ticket] = []
    
    @IBOutlet weak var cancelOutlet: UIButton!
    @IBAction func cancelPressed(sender: AnyObject) {
        let con = MySQL.Connection()
        let db_name = "sql6133445"
        
        do{
            
            // open a new connection
            try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
            
            try con.use(db_name)
            
            let select_stmt = try con.prepare("SELECT * FROM Tickets WHERE TekTokker = '"+GVar.fullName+"'")
            let res = try select_stmt.query([])
            let rows = try res.readAllRows()
            
            //"UPDATE `sql6112602`.`Tickets` SET `Cancelled` = '1' AND `TekTokker` = '' WHERE `Tickets`.`TeacherName` = '{0}' LIMIT 1 ; UPDATE `sql6112602`.`Users` SET `Hired` = false WHERE Name = '{1}'", aTicket["TeacherName"], LoginActivity._User.Name
            try con.query("UPDATE Tickets SET Cancelled = '1', TekTokker = '' WHERE TeacherName = '" + String(rows![0][0]["TeacherName"]!) + "' LIMIT 1")
            let name = GVar.fullName
            try con.query("UPDATE Users SET Hired = '0' WHERE Name = '" + name + "'")
            
        }
        catch(let e){
            print(e)
        }
        
    }
    @IBAction func unwindToTekTokker(segue: UIStoryboardSegue) {
    }
    
    @IBOutlet weak var finishPressed: UIButton!
    @IBAction func finishPressed(sender: AnyObject) {
        let con = MySQL.Connection()
        let db_name = "sql6133445"
        
        do{
            
            // open a new connection
            try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
            
            try con.use(db_name)
            
            
            
            let select_stmt = try con.prepare("SELECT * FROM Tickets WHERE TekTokker = '"+GVar.fullName+"'")
            let res = try select_stmt.query([])
            let rows = try res.readAllRows()
            
            //"UPDATE `sql6112602`.`Tickets` SET `Cancelled` = '1' AND `TekTokker` = '' WHERE `Tickets`.`TeacherName` = '{0}' LIMIT 1 ; UPDATE `sql6112602`.`Users` SET `Hired` = false WHERE Name = '{1}'", aTicket["TeacherName"], LoginActivity._User.Name
            try con.query("UPDATE Tickets SET Finished = '1', TekTokker = '' WHERE TeacherName = '" + String(rows![0][0]["TeacherName"]!) + "' LIMIT 1")
            let name = GVar.fullName
            try con.query("UPDATE Users SET Hired = '0' WHERE Name = '" + name + "'")
            
        }
        catch(let e){
            print(e)
        }
        
    }
    
    @IBOutlet weak var finishOutlet: UIButton!
    
    override func viewDidLoad() {
        
        super.viewDidLoad()
        self.navigationItem.hidesBackButton = true;
        let message = "Welcome, " + GVar.givenName
        self.cancelOutlet.hidden = true
        self.finishOutlet.hidden = true
        self.welcomeMessage.text = message
        self.buttonOut.hidden = true
        self.Extrainfooutlet.numberOfLines = 0
        
    }
    
    
    override func viewDidAppear(animated: Bool){
        
        dispatch_async(dispatch_get_global_queue(QOS_CLASS_BACKGROUND, 0), {
            
            let taskID = self.beginBackgroundUpdateTask()
            
            let con = MySQL.Connection()
            let db_name = "sql6133445"
            do{
                // open a new connection
                try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
                
                try con.use(db_name)
                var notiftickets:[Ticket] = []
                
                while(GVar.keepChecking){
                    let select_stmt = try con.query("SELECT * FROM Users WHERE Email = '"+GVar.email+"'")
                    let rows = try select_stmt.readAllRows()
                    
                    print("bg test")
                    if(String(rows![0][0]["Hired"]!) == "0"){
                        
                        dispatch_async(dispatch_get_main_queue()) {
                            
                            
                            self.TeacherNameOutlet.hidden = true
                            self.Extrainfooutlet.hidden = true
                            self.cancelOutlet.hidden = true
                            self.finishOutlet.hidden = true
                            self.readyOutlet.text = "Ready to lend a helping hand!"
                        }
                        
                        let select_stmt = try con.prepare("SELECT * FROM Tickets")
                        let res = try select_stmt.query([])
                        //let select_stmt = try con.query("SELECT * FROM Tickets WHERE TeacherName =?")
                        let rows = try res.readAllRows()
                        
                        if((rows!.isEmpty)){
                            dispatch_async(dispatch_get_main_queue()) {
                                self.buttonOut.hidden = true
                                self.AwaitImage.hidden = false
                            }
                        }
                        else{
                            
                            for row in rows![0]{
                                if(String(row["Accepted"]!) == "0"){
                                    
                                    
                                    
                                    
                                    let newticket = Ticket()
                                    newticket.teachername = String(row["TeacherName"]!)
                                    newticket.room = String(row["Room"]!)
                                    newticket.extraInfo = String(row["ExtraInformation"]!)
                                    self.tickets.append(newticket)
                                    
                                }
                            }
                            
                            if(self.tickets.count > 0){
                                
                                for ticket in self.tickets{
                                    if !notiftickets.contains(ticket) {
                                        
                                    
                                        notiftickets.append(ticket)
                                        guard let settings = UIApplication.sharedApplication().currentUserNotificationSettings() else{ continue }
                                        
                                        if settings.types == .None {
                                            let ac = UIAlertController(title: "Please allow notifications!", message: "No permission to schedule Notification", preferredStyle: .Alert)
                                            ac.addAction(UIAlertAction(title: "OK", style: .Default, handler: nil))
                                            dispatch_async(dispatch_get_main_queue()) {
                                                self.presentViewController(ac, animated: true, completion: nil)
                                            }
                                        }
                                        else{
                                            print("notif")
                                            let notification = UILocalNotification()
                                            notification.alertBody = String(ticket.teachername) + " needs help @" + String(ticket.room)
                                            notification.alertAction = "open" // text that is displayed after "slide to..." on the lock screen - defaults to "slide to view"
                                            notification.fireDate = NSDate(timeIntervalSinceNow: 5)
                                            notification.soundName = UILocalNotificationDefaultSoundName // play default sound
                                            
                                            UIApplication.sharedApplication().scheduleLocalNotification(notification)
                                            
                                        }
                                    }
                                    
                                }
                               
                                dispatch_async(dispatch_get_main_queue()) {
                                    self.buttonOut.hidden = false
                                    self.AwaitImage.hidden = true
                                }
                                //notify
                               
                            }else{
                                print("something should be happening")
                                dispatch_async(dispatch_get_main_queue()) {
                                    self.buttonOut.hidden = true
                                    self.AwaitImage.hidden = false
                                }
                            }
                            
                            
                        }
                        
                    }
                    
                    
                    
                    
                    if(String(rows![0][0]["Hired"]!) == "1"){
                        dispatch_async(dispatch_get_main_queue()) {
                            self.cancelOutlet.hidden = false
                            self.finishOutlet.hidden = false
                            self.buttonOut.hidden = true
                            self.AwaitImage.hidden = true
                        }
                        
                        /*
                         let select_stmt = try con.prepare("SELECT * FROM Tickets")
                         let res = try select_stmt.query([])
                         //let select_stmt = try con.query("SELECT * FROM Tickets WHERE TeacherName =?")
                         let rows = try res.readAllRows()
                         
                         */
                        let select_stmt = try con.prepare("SELECT * FROM Tickets WHERE TekTokker = '"+GVar.fullName+"'")
                        let res = try select_stmt.query([])
                        let rows = try res.readAllRows()
                        if((rows!.isEmpty) == false){
                            if(String(rows![0][0]["TimedOut"]!) == "1"){
                                //alert
                                dispatch_async(dispatch_get_main_queue()) {
                                    let myAlert = UIAlertController(title: "Ticket Timed Out", message: "Your Ticket has timed out", preferredStyle: UIAlertControllerStyle.Alert);
                                    let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                                        
                                        //myAlert.dismissViewControllerAnimated(false, completion: nil)
                                        do{
                                            try con.query("DELETE FROM Tickets WHERE TekTokker = '" + GVar.fullName + "'")
                                        }catch(let e){
                                            print(e)
                                        }
                                        
                                        
                                    }
                                    myAlert.addAction(okAction);
                                    self.presentViewController(myAlert, animated: true, completion: nil)
                                }

                                //notify
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
                                
                            }
                            else{
                                dispatch_async(dispatch_get_main_queue()) {
                                    self.TeacherNameOutlet.text = "On the job for: " + String(rows![0][0]["TeacherName"]!)
                                    self.readyOutlet.text = "@ " + String(rows![0][0]["Room"]!)
                                    self.Extrainfooutlet.text = "Extra Info: " + String(rows![0][0]["ExtraInformation"]!)
                                    self.TeacherNameOutlet.hidden = false
                                    self.Extrainfooutlet.hidden = false
                                }
                            }
                            
                            sleep(1)
                        }
                        else{
                            let select_stmt = try con.prepare("SELECT Hired FROM Users WHERE Name = '"+GVar.fullName+"'")
                            let res = try select_stmt.query([])
                            let rows = try res.readAllRows()
                            if(String(rows![0][0]["Hired"]!) == "1"){
                                try con.query("UPDATE Users SET Hired = 0 WHERE Name = '"+GVar.fullName+"'")
                                
                                dispatch_async(dispatch_get_main_queue()) {
                                    let myAlert = UIAlertController(title: "Ticket Cancelled", message: "Ticket has been cancelled by requester", preferredStyle: UIAlertControllerStyle.Alert);
                                    let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                                        
                                    }
                                    myAlert.addAction(okAction);
                                    self.presentViewController(myAlert, animated: true, completion: nil)
                                    
                                    
                                    
                                }
                                
                                guard let settings = UIApplication.sharedApplication().currentUserNotificationSettings() else{ continue }
                                
                                if settings.types == .None {
                                    let ac = UIAlertController(title: "Can't schedule", message: "Either we don't have permission to schedule notifications, or we haven't asked yet.", preferredStyle: .Alert)
                                    ac.addAction(UIAlertAction(title: "OK", style: .Default, handler: nil))
                                    dispatch_async(dispatch_get_main_queue()) {
                                        self.presentViewController(ac, animated: true, completion: nil)
                                    }
                                }
                                else{
                                    print("notif")
                                    let notification = UILocalNotification()
                                    notification.alertBody = "Ticket has been cancelled by requester!" // text that will be displayed in the notification
                                    notification.alertAction = "open" // text that is displayed after "slide to..." on the lock screen - defaults to "slide to view"
                                    notification.fireDate = NSDate(timeIntervalSinceNow: 5)
                                    notification.soundName = UILocalNotificationDefaultSoundName // play default sound
                                    
                                    UIApplication.sharedApplication().scheduleLocalNotification(notification)
                                    
                                }
                                
                                
                                
                            }
                            
                            
                            //notif
                        }
                    }
                    
                    GVar.tickets = self.tickets
                    self.tickets = self.emptytickets
                    sleep(5)
                }
                
                
                
                try con.close()
            }
            catch (let e) {
                self.buttonOut.hidden = false
                print(e)
            }
            self.endBackgroundUpdateTask(taskID)
        })
        
        
        
    }
    
    func beginBackgroundUpdateTask() -> UIBackgroundTaskIdentifier {
        return UIApplication.sharedApplication().beginBackgroundTaskWithExpirationHandler({})
    }
    
    func endBackgroundUpdateTask(taskID: UIBackgroundTaskIdentifier) {
        UIApplication.sharedApplication().endBackgroundTask(taskID)
    }
    
    
    @IBAction func button(sender: AnyObject) {
        dispatch_async(dispatch_get_main_queue()) {
            self.performSegueWithIdentifier("toTable", sender: nil)
        }
    }
    
    
    
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    
    
}
