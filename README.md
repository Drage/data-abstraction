# data-abstraction
Provides an object-oriented interface that abstracts the interaction with a relational database via SQL

## Purpose
This code tests the idea of removing the need to write SQL code for common tasks such as loading a list of records from a table, updating, and deleting them.

This consists of a set of attributes that define the mapping between classes and database tables, as well as properties and database fields (columns). These are used to add the necessary metadata to to allow for the SQL queries to be automatically generated. 

## Usage
### Defining a Data Base Object (DBO)
```
[DataTable("tblAppointments")]
class Appointment : DatabaseObject
{
    private int id;
    private string description;

    [PrimaryKey("AppointmentID")]
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    [DataField("Desc")]
    public string Description
    {
        get { return description; }
        set { description = value; }
    }
    
    [DataField("Status")]
    public string Status
    {
        get { return status; }
        set { status = value; }
    }
}
```

### Loading a list of DBOs
```
// Load all
// SQL: SELECT AppointmentID FROM tblAppointments
List<Appointment> appointments = DatabaseObject.Load<Appointment>(connection);

// Load with filter on status
// SQL: SELECT AppointmentID FROM tblAppointments WHERE Status='Booked'
QueryFilter filter = new QueryFilter("Status", "Booked", QueryFilter.ComparisonType.Equals);
List<Appointment> appointments = DatabaseObject.Load<Appointment>(connection, [filter]);
```

### Updating a DBO
```
Appointment appt = appointments[0];
appt.Description = 'Updated description';
appt.Save();
// SQL: UPDATE tblAppointments SET Description='Updated description', Status='Booked' WHERE AppointmentID=1
```
