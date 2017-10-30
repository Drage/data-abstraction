
namespace DataAbstraction
{
    /**
     * @brief   Filters query results by property.
     * @author  Matt Drage
     * @date    22/06/2012
     */
    public class QueryFilter : QueryModifier
    {
        public enum ComparisonType { Like, Equals, GreaterThan, LessThan }

        public string Property;
        public object Filter;
        public ComparisonType CompareType = ComparisonType.Like;
        public bool Negation = false;
        protected internal string Field;

        /**
         * @brief   Default constructor.
         * @author  Matt Drage
         * @date    22/06/2012
         */
        public QueryFilter() { }

        /**
         * @brief   Constructor.
         * @author  Matt Drage
         * @date    22/06/2012
         * @param   property    The property to filter by.
         * @param   filter      The filter to match results to.
         * @param   negation    (optional) True to negate filter (do opposite).
         */
        public QueryFilter(string property, object filter, ComparisonType compareType = ComparisonType.Like, bool negation = false)
        {
            Property = property;
            Filter = filter;
            CompareType = compareType;
            Negation = negation;
        }
    }
}
