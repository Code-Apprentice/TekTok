//
//  TektokkerViewController.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 09/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit

class TektokeeViewController: UIViewController {
    
    var image1: UIImage!
    var image2: UIImage!
    var image3: UIImage!
    var image4: UIImage!
    var image5: UIImage!
    var image6: UIImage!
    
    @IBOutlet weak var helpRequested: UIButton!
    var image7: UIImage!
    @IBAction func logout(sender: AnyObject) {
        GIDSignIn.sharedInstance().signOut()
        navigationController?.popToRootViewControllerAnimated(true)

    }
    var images: [UIImage]!
    var animate: UIImage!
    
    @IBOutlet weak var animation: UIImageView!
    @IBOutlet weak var welcomeMessage: UILabel!
    override func viewDidLoad() {
        
        
        super.viewDidLoad()
        self.navigationItem.hidesBackButton = true;
        self.welcomeMessage.text = "Welcome, " + GVar.givenName
        
        image1 = UIImage(named: "o_d5c638efe3964d85-0.jpg")
        image2 = UIImage(named: "o_d5c638efe3964d85-1.jpg")
        image3 = UIImage(named: "o_d5c638efe3964d85-2.jpg")
        image4 = UIImage(named: "o_d5c638efe3964d85-3.jpg")
        image5 = UIImage(named: "o_d5c638efe3964d85-4.jpg")
        image6 = UIImage(named: "o_d5c638efe3964d85-5.jpg")
        image7 = UIImage(named: "o_d5c638efe3964d85-6.jpg")

        images = [image1, image2,image3,image4,image5,image6,image7]
        animate = UIImage.animatedImageWithImages(images, duration: 3)
        animation.hidden = true;
        
        
        
        
        
        
        // Do any additional setup after loading the view.
    }
    var checkBool = true
    @IBAction func unwindToTekTokee(segue: UIStoryboardSegue) {
    }
    
    override func viewDidAppear(animated: Bool) {
        
        
        
            
        let con = MySQL.Connection()
        let db_name = "sql6133445"
        do{
            try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
            try con.use(db_name)
            let select_stmt = try con.prepare("SELECT * FROM Tickets WHERE TeacherName = '"+GVar.fullName+"'")
            let res = try select_stmt.query([])
            let rows = try res.readAllRows()!
            if(rows.isEmpty == false){
                GVar.requested = true
            }
        }catch(let e){
            print(e)
        }
        
       

        
        
        
        
    
        if(GVar.requested){
            self.welcomeMessage.text = "Requesting for Help"
            helpRequested.hidden = true
            animation.image = animate
            animation.hidden = false
            
            
            
            dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), {
                
                
            let taskID = self.beginBackgroundUpdateTask()
                
            NSTimer.scheduledTimerWithTimeInterval(60, target: self, selector: #selector(TektokeeViewController.update(_:)), userInfo: nil, repeats: false)
            
            
            
                
            while(self.checkBool){
                
                do{
                    /*
                     SELECT column_name FROM table_name
                     ORDER BY column_name DESC
                     LIMIT 1;
                    */
                    
                    let select_stmt = try con.prepare("SELECT Accepted, TekTokker FROM Tickets WHERE TeacherName = '"+GVar.fullName+"' ORDER BY Accepted DESC LIMIT 1")
                    let res = try select_stmt.query([])
                    let rows = try res.readAllRows()
                    print("Check Accepted")
                    if((!(rows?.isEmpty)! && String(rows![0][0]["Accepted"]) == "Optional(1)")){
                        dispatch_async(dispatch_get_main_queue()) {
                            self.welcomeMessage.text = "Welcome, " + GVar.givenName
                            self.helpRequested.hidden = false
                            self.animation.hidden = true
                        
                            do{
                                try con.close()
                            }catch(let e){
                                print(e)
                            }
                            
                            GVar.tektokker = String(rows![0][0]["TekTokker"]!)
                            GVar.requested = false
                            print("lol")
                            self.performSegueWithIdentifier("totekticket", sender: nil)
                        }
                    }
                    if(GVar.requested == false){
                        
                        break
                    }
                    
                    
                    
                }
                catch(let e){
                    print(e)
                    break
                }
                
                
                
            }
            
            self.endBackgroundUpdateTask(taskID)
                do{
                    try con.close()
                }catch(let e){
                    print(e)
                }
            
            })
            
            
            
        }
        else{
            self.welcomeMessage.text = "Welcome, " + GVar.givenName
            helpRequested.hidden = false
            animation.image = animate
            animation.hidden = true
        }
        
    }
    
    func beginBackgroundUpdateTask() -> UIBackgroundTaskIdentifier {
        return UIApplication.sharedApplication().beginBackgroundTaskWithExpirationHandler({})
    }
    
    func endBackgroundUpdateTask(taskID: UIBackgroundTaskIdentifier) {
        UIApplication.sharedApplication().endBackgroundTask(taskID)
    }
    
    func update(sender: AnyObject){
        GVar.requested = false
        checkBool = false
    }
    
    @IBAction func helpReqAction(sender: AnyObject) {
        performSegueWithIdentifier("toTicketReq", sender: nil)
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
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
