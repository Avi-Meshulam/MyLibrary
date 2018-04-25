# MyLibrary
### A WPF application, simulating a book library, incorporating OOP principles and binary serialization.
![Filtering](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/Filtering.gif "Filtering")

The application consists of 3 projects:
+ MyLibrary - UI gateway
+ MyLibrary.BL - Business Logic Layer
+ MyLibrary.DAL - Data Access Layer

### Features/Technologies/Tools:
+ WPF
+ Object Oriented Programming
+ 3 Layers Architecture
+ Interfaces & Generics
+ Custom Observable Collections
+ Custom 'Window' objects
+ Data Grids & Filtering
+ Toolbars
+ User & Permissions
+ Binary Serialization
+ Isolated Storage

### Login Instructions
+ On first launch, user name & password will be auto filled with admin/admin.
+ On the top-right corner you will see a "Generate Demo Data" button - click it to load demo data. The button apears only as long as there's no data in storage. Demo data includes, among the rest, 20 usersusers, with the following credentials:
+ User name: "user{01-20}"
+ Password: "pass{01-20}"

### Business Logic Models
#### Entities - Overview
![Models Overview](https://github.com/PrisonerM13/MyLibrary/blob/master/images/ModelsOverview.png "Models Overview")

#### Entities - Details
![Models Details](https://github.com/PrisonerM13/MyLibrary/blob/master/images/ModelsDetails.png "Models Details")

#### Entity Manager
![Entity Manager](https://github.com/PrisonerM13/MyLibrary/blob/master/images/EntityManager.png "Entity Manager")

### UI Models
#### Entities Window
![Entities Windows](https://github.com/PrisonerM13/MyLibrary/blob/master/images/EntitiesWindow.png "Entities Window")

#### Entity Window
![Entity Window](https://github.com/PrisonerM13/MyLibrary/blob/master/images/EntityWindow.png "Entity Window")

### Isolated Storage
#### Root directory format
%LOCALAPPDATA%\IsolatedStorage\{xxxxxxxx}.ldz\{xxxxxxxx}.mrf\Url.{GUID}\AssemFiles
		
#### Folders (i.e 'tables')
+ Employees
+ LibraryItems
+ Publishers
+ Users

### Operations

+ Add Item
		
	![Add Item](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddItem.gif "Add Item")

+ Add Copies & Borrow
		
	![Add Copies And Borrow](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddCopiesAndBorrow.gif "Add Copies And Borrow")

+ ISBN Filtering
		
	![ISBN Filtering](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/ISBNFiltering.gif "ISBN Filtering")

+ Advanced Search
		
	![Advanced Search](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AdvancedSearch.gif "Advanced Search")

+ Add Employee
		
	![Add Employee](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddEmployee.gif "Add Employee")

+ Add User
		
	![Add User](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddUser.gif "Add User")

+ Permission
	+ Manager
		
		![Manager Permissions](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/ManagerPermissions.gif "Manager Permissions")
	+ Supervisor - Cannot add/edit employees and users.
		
		![Supervisor Permissions](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/SupervisorPermissions.gif "Supervisor Permissions")

	+ Worker - Cannot add/edit employees, users, publishers and books.
		
		![Worker Permissions](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/WorkerPermissions.gif "Worker Permissions")
