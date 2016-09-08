//
//  TicketInfoViewController.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 14/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit

class TicketInfoViewController: UIViewController {

    //shows ticket information
    //oreintation stuff
    override func shouldAutorotate() -> Bool {
        return false
    }
    
    override func supportedInterfaceOrientations() -> UIInterfaceOrientationMask {
        return UIInterfaceOrientationMask.Portrait
    }
    //new ticket object
    var ticket = Ticket()
    
    @IBOutlet weak var teacherNameOutlet: UILabel!
    @IBOutlet weak var roomOutlet: UILabel!
    @IBOutlet weak var extraInformationOutet: UILabel!
    @IBAction func acceptAction(sender: AnyObject) {
        
        
        //connection stuffs
        let con = MySQL.Connection()
        let db_name = "sql6133445"
        
        do{
            
            // open a new connection
            try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
            
            try con.use(db_name)
            //update ticket object
            
            try con.query("UPDATE Tickets SET Accepted = '1', TekTokker = '" + GVar.fullName + "' WHERE TeacherName = '"+ticket.teachername+"' LIMIT 1")
            try con.query("UPDATE Users SET Hired = '1', Email = '" + GVar.email + "' LIMIT 1")
            
            
            //segue back to the tektokker main menu
            performSegueWithIdentifier("backToTekTokker", sender: nil)
        }
        catch(let e){
            print(e)
        }
        
    }
    override func viewDidLoad() {
        
        //show info
        super.viewDidLoad()
        teacherNameOutlet.text = ticket.teachername
        roomOutlet.text = ticket.room
        extraInformationOutet.text = ticket.extraInfo
        extraInformationOutet.numberOfLines = 0
    }
    
    override func viewDidAppear(animated: Bool) {
        //check if already accepted
    }
    
    
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    
    
}
