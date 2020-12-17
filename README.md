
# ContactsAPI
Simple Contacts &amp; Skills Crud API using Asp.Net Core 5

# Installation

To create the database you will need to run the following command in the Package Manager Console (having selected the ContactsAPI.Data project): 
    // Update-Database
	
But before doing this it's a good idea to verify the connection string in the appsettings to verify if it's pointing to the correct sql server instance.
Below you can find the default connection string:
"ConnectionStrings": {
        "ContactsApiDbContext": "Server=localhost; Database=ContactsApi; Trusted_Connection=true; MultipleActiveResultSets=true; Integrated Security=true;"
    }	

# Authentication and Authorization
The method used for authenticaion here is JWT tokens.

The CRUD operations for Contacts and ContactSkill need authentication. This is necessary to verify if the user trying to make changes in these 2 tables is authorized to do so; users can only change their contact and the current user can’t change skills for other users.

To get the JWT token necessary to invoke authenticated web methods, you can make a call like:
https://localhost:44384/api/user/authenticate
{
   "Username": "jl",
   "Password": "G78p1gY!"
}
Of course before running this we need to create the user and it's contact. To do this we can run (POST) something like:
https://localhost:44384/api/user/register
{
   "Username": "jl",
   "Password": "G78p1gY!",

   "Firstname": "John",
   "Lastname": "Lewisham",

   "AddressLine1": "3 Myrtle Walk",
   "AddressLine2": "Hawk House",
   "City": "London",
   "PostalCode": "75015",
   "Country": "UK",
   
   "Email": "jl@gmail.com",
   "MobilePhoneNumber": "+44 765454457"
}

Note: The idea here is that users and contacts are inserted without any skills and
the skills are added later by invoking the create contactSkill in the api.
Of course before calling the contactSkill create web method to say which skills a contact has,
we must have created an entry in the Skills table before using the create Skill web method.
Users, Contacts, Skills and ContactSkills tables are all using integer primary keys.

If you're using Postman to test, you can select "Bearer Token" in the "Authorization" tab and insert the given toke there.

# Other example calls
Other example of calls would be:

// Contact  (POST)
https://localhost:44384/api/contact
{
   "Firstname": "Peverell",
   "Lastname": "Lefèbvre",

   "AddressLine1": "28 Rue La Boétie",
   "AddressLine2": "Île-de-France",
   "City": "Paris",
   "PostalCode": "75015",
   "Country": "France",
   
   "Email": "pevlef@gmail.com",
   "MobilePhoneNumber": "+33 765454457"
}		



// Insert Skill  (POST)
https://localhost:44384/api/skill
{
   "Name": "C#"
}



// Insert ContactSkill  (POST)
https://localhost:44384/api/contactSkill
{
   "ContactId": "1",
   "SkillId": "1",
   "Level": "9"
}		
	

To update all fields in a contact record  (PUT)
https://localhost:44384/api/contact
{
   "Id": "1",
   "Firstname": "X",
   "Lastname": "Y",

   "AddressLine1": "28 Rue La Boétie",
   "AddressLine2": "Île-de-France",
   "City": "Paris",
   "PostalCode": "75015",
   "Country": "France",
   
   "Email": "pevlef@gmail.com",
   "MobilePhoneNumber": "+33 765454457"
}		

// To update some fields in a contact record (PATCH)
https://localhost:44384/api/contact/1
[
   {  
     "value": "28 Rue La Liberté",
     "path": "/addressLine1",
	 "op": "replace"
   }
]

// Delete Skill  (DELETE)
https://localhost:44384/api/skill/1


