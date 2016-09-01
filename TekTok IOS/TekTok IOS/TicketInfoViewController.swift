//
//  TicketInfoViewController.swift
//  TekTok IOS
//
//  Created by Zhi Wei Gan on 14/08/2016.
//  Copyright © 2016 Zhi Wei Gan. All rights reserved.
//

import UIKit

class TicketInfoViewController: UIViewController {

    var ticket = Ticket()
    
    @IBOutlet weak var teacherNameOutlet: UILabel!
    @IBOutlet weak var roomOutlet: UILabel!
    @IBOutlet weak var extraInformationOutet: UILabel!
    @IBAction func acceptAction(sender: AnyObject) {
        
        let con = MySQL.Connection()
        let db_name = "sql6133445"
        
        do{
            
            // open a new connection
            try con.open("sql6.freemysqlhosting.net", user: "sql6133445", passwd: "hhh6WUDEWP")
            
            try con.use(db_name)
            
            try con.query("UPDATE Tickets SET Accepted = '1', TekTokker = '" + GVar.fullName + "' WHERE TeacherName = '"+ticket.teachername+"' LIMIT 1")
            try con.query("UPDATE Users SET Hired = '1', Email = '" + GVar.email + "' LIMIT 1")
            
            performSegueWithIdentifier("backToTekTokker", sender: nil)
        }
        catch(let e){
            print(e)
        }
        
     //"UPDATE `sql6112602`.`Tickets` SET `Accepted` = '1', `TekTokker` = '" + LoginActivity._User.Name + "'" + " WHERE `Tickets`.`TeacherName` = '" + name + "'" + " LIMIT 1 ;UPDATE `sql6112602`.`Users` SET `Hired` = '1' WHERE `Users`.`Email` = '" + LoginActivity._User.Email + "'" + " LIMIT 1 ;"
    }
    override func viewDidLoad() {
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
