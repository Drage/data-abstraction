
namespace DataAbstraction
{
    /**
     * @brief   Defines the field in a database record that a property relates to. 
     * @author  Matt Drage
     * @date    21/06/2012
     */
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = true)]
    public class DataField : System.Attribute
    {
        private string name;

        /**
         * @brief   Constructor.
         * @author  Matt Drage
         * @date    21/06/2012
         * @param   name    The name of the database table field.
         */
        public DataField(string name)
        {
            this.name = name;
        }

        /**
         * @brief   Gets the name of the field.
         * @return  The name.
         */
        public string Name
        {
            get { return name; }
        }
    }
}
