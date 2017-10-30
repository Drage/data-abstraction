
namespace DataAbstraction
{
    /**
     * @brief   Values that represent sorting orders. 
     */
    public enum SortDirection { Ascending, Descending };

    /**
     * @brief   Sorts records returned by a database object query. 
     * @author  Matt Drage
     * @date    22/06/2012
     */
    public class QuerySort : QueryModifier
    {
        public string Property;
        public SortDirection Direction;
        protected internal string Field;

        /**
         * @brief   Default constructor.
         * @author  Matt Drage
         * @date    22/06/2012
         */
        public QuerySort() { }

        /**
         * @brief   Constructor.
         * @author  Matt Drage
         * @date    27/06/2012
         * @param   property    The property to sort by.
         * @param   direction   The direction to sort by.
         */
        public QuerySort(string property, SortDirection direction)
        {
            Property = property;
            Direction = direction;
        }
    }
}
