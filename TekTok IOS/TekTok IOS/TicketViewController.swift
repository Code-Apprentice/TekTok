//
//  TicketViewController.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 12/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit

class TicketViewController: UIViewController, UITextViewDelegate,  UITextFieldDelegate {

    override func viewDidLoad() {
        super.viewDidLoad()

        self.navigationItem.hidesBackButton = true
        let newBackButton = UIBarButtonItem(title: "Back", style: UIBarButtonItemStyle.Bordered, target: self, action: #selector(TicketViewController.back(_:)))
        self.navigationItem.leftBarButtonItem = newBackButton;
        ExtraText.delegate = self
        RoomText.delegate = self
        // Do any additional setup after loading the view.
    }
    var roomID = ""
    var extraInfo = ""
    @IBOutlet weak var ExtraText: UITextView!
    @IBOutlet weak var RoomText: UITextField!
    
    func textFieldShouldReturn(textField: UITextField) -> Bool {
        self.view.endEditing(true)
        return false
    }
    
    func textView(textView: UITextView, shouldChangeTextInRange range: NSRange, replacementText text: String) -> Bool {
        if text == "\n"  // Recognizes enter key in keyboard
        {
            textView.endEditing(true)
            return false
        }
        return true
    }

    @IBAction func RoomFinished(sender: AnyObject) {
        roomID = RoomText.text!
    }
    @IBAction func SendTicket(sender: AnyObject) {
        extraInfo = ExtraText.text
        if(extraInfo == "" || roomID == ""){
            let myAlert = UIAlertController(title: "Form Invalid", message: "You have not completed the form!", preferredStyle: UIAlertControllerStyle.Alert);
            let okAction = UIAlertAction(title: "Ok", style: UIAlertActionStyle.Default){(ACTION) in
                
            }
            myAlert.addAction(okAction);

            presentViewController(myAlert, animated: true, completion: nil)
        }
        else{
            let con = MySQL.Connection()
            let db_name = "sql6133445"
            do{
                
                try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
                
                try con.use(db_name)
                
                let ins_stmt = try con.prepare("INSERT INTO Tickets(TeacherName, Room, ExtraInformation, Accepted, Finished, Cancelled) VALUES(?,?,?,?,?,?)")
                try ins_stmt.exec([GVar.fullName, roomID, extraInfo, "0", "0","0"])
                GVar.requested = true
                try con.close()
                
                navigationController?.popViewControllerAnimated(true)

                
            }catch (let e){
                print(e)
            }
            
        }
    }
    
    func back(sender: UIBarButtonItem) {
        // Perform your custom actions
        // ...
        // Go back to the previous ViewController
        let myAlert = UIAlertController(title: "Cancel Request?", message: "Do you want to cancel your request?", preferredStyle: UIAlertControllerStyle.Alert);
        let okAction = UIAlertAction(title: "Yes", style: UIAlertActionStyle.Default){(ACTION) in
            self.roomID = ""
            self.extraInfo = ""
            self.navigationController?.popViewControllerAnimated(true)

        }
        let noAction = UIAlertAction(title: "No", style: UIAlertActionStyle.Default){(ACTION) in
        }
        myAlert.addAction(okAction)
        myAlert.addAction(noAction)
        presentViewController(myAlert, animated: true, completion: nil)
        
        self.navigationController?.popViewControllerAnimated(true)
    }

    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    

    /*
     
     override func viewDidLoad {
     super.viewDidLoad()
     self.navigationItem.hidesBackButton = true
     let newBackButton = UIBarButtonItem(title: "Back", style: UIBarButtonItemStyle.Bordered, target: self, action: "back:")
     self.navigationItem.leftBarButtonItem = newBackButton;
     }
     
     func back(sender: UIBarButtonItem) {
     // Perform your custom actions
     // ...
     // Go back to the previous ViewController
     self.navigationController?.popViewControllerAnimated(true)
     }
     
     */
    
    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepareForSegue(segue: UIStoryboardSegue, sender: AnyObject?) {
        // Get the new view controller using segue.destinationViewController.
        // Pass the selected object to the new view controller.
    }
    */

}
