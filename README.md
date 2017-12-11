![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/Filtering.gif "Filtering")

The application consists of 3 projects:
+ MyLibrary - UI gateway
+ MyLibrary.BL - Business Logic Layer
+ MyLibrary.DAL - Data Access Layer

### Features/Technologies/Tools:
+ WPF
+ Object Oriented Programming
+ 3 Layers Design
+ Interfaces & Generics
+ Observable Collections & Events
+ UI components reuse
+ Data Grids
+ Filtering
+ Toolbars
+ User Permissions
+ Isolated Storage
+ Binary Serialization

### Login Instructions
+ On first launch user name & password will be auto filled with admin/admin.
+ On the top-right corner you will see a "Generate Demo Data" button - click it to load demo data.
+ "Generate Demo Data" button will be displayed only as long as there's no data in storage.
+ "Generate Demo Data" button creates 20 users: user[01-20]/pass[01-20] respectively.

### Business Logic Highlights
#### Models - Overview
![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/images/ModelsOverview.png "Models Overview")

#### Models - Details
![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/images/ModelsDetails.png "Models Details")

#### Entity Manager
![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/images/EntityManager.png "Entity Manager")

### UI Highlights
#### Entities Window
![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/images/EntitiesWindow.png "Entities Window")

#### Entity Window
![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/images/EntityWindow.png "Entity Window")

### Isolated Storage
#### Root directory format
%LOCALAPPDATA%\IsolatedStorage\{xxxxxxxx}.ldz\{xxxxxxxx}.mrf\Url.{xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx}\AssemFiles
		
#### Folders (i.e 'tables')
+ Employees
+ LibraryItems
+ Publishers
+ Users

### Operations

+ Add Item
		
	![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddItem.gif "Add Item")

+ Add Copies & Borrow
		
	![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddCopies.gif "Add Copies")

+ ISBN Filtering
		
	![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/ISBNFiltering.gif "ISBN Filtering")

+ Advanced Search
		
	![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AdvancedSearch.gif "Advanced Search")

+ Add Employee
		
	![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddEmployee.gif "Add Employee")

+ Add User
		
	![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/AddUser.gif "Add User")

+ Permissions
	+ Manager
		
		![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/ManagerPermissions.gif "Manager Permissions")

	+ Supervisor
		
		![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/SupervisorPermissions.gif "Supervisor Permissions")

	+ Worker
		
		![alt text](https://github.com/PrisonerM13/MyLibrary/blob/master/gif/WorkerPermissions.gif "Worker Permissions")
