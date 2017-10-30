
namespace DataAbstraction
{
    /**
     * @brief   Defines the database table a DBO belongs to. 
     * @author  Matt Drage
     * @date    21/06/2012
     */
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class DataTable : System.Attribute
    {
        private string name;

        /**
         * @brief   Constructor.
         * @author  Matt Drage
         * @date    21/06/2012
         * @param   name    The name of the table.
         */
        public DataTable(string name)
        {
            this.name = name;
        }

        /**
         * @brief   Gets the name of the table.
         * @return  The name.
         */
        public string Name
        {
            get { return name; }
        }
    }
}
