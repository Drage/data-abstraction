
namespace DataAbstraction
{
    /**
     * @brief   Defines the primary key in a database record that a property relates to. 
     * @author  Matt Drage
     * @date    21/06/2012
     */
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class PrimaryKey : System.Attribute
    {
        private string name;

        /**
         * @brief   Constructor.
         * @author  Matt Drage
         * @date    21/06/2012
         * @param   name    The name of the primary key.
         */
        public PrimaryKey(string name)
        {
            this.name = name;
        }

        /**
         * @brief   Gets the name of the primary key.
         * @return  The name.
         */
        public string Name
        {
            get { return name; }
        }
    }
}
