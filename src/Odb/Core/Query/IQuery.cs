using System;
using System.Collections.Generic;
using NDatabase2.Odb.Core.Query.Criteria;

namespace NDatabase2.Odb.Core.Query
{
    /// <summary>
    /// handle to a node in the query graph.
    /// 
    /// A node in the query graph can represent multiple 
    /// classes, one class or an attribute of a class.
    /// 
    /// The graph 
    /// is automatically extended with attributes of added constraints 
    /// (see constrain()) and upon calls to  descend()
    /// that request nodes that do not yet exist.
    /// 
    /// References to joined nodes in the query graph kann be obtained
    /// by "walking" along the nodes of the graph with the method 
    /// descend().
    /// 
    /// execute()
    /// evaluates the entire graph against all persistent objects. 
    /// 
    /// execute() can be called from any Query node
    /// of the graph. It will return an ObjectSet filled with
    /// objects of the class/classes that the node, it was called from,
    /// represents.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// adds a constraint to this node.
        /// 
        /// If the constraint contains attributes that are not yet
        /// present in the query graph, the query graph is extended
        /// accordingly.
        /// </summary>
        /// <param name="value">constraint the constraint to be added to this Query.</param>
        /// <returns>Constraint a new Constraint for this query node.</returns>
        IConstraint Constrain(object value);


        /// <summary>
        /// executes the Query.
        /// </summary>
        /// <returns>ObjectSet - the result of the Query.</returns>
        IObjectSet<T> Execute<T>() where T : class;


        /// <summary>
        /// executes the Query.
        /// </summary>
        /// <param name="inMemory">Indicates if all returned data should be loaded to memory (inMemory is true)
        /// or if the data should be lazy loaded (inMemory to false)</param>
        /// <returns>ObjectSet - the result of the Query.</returns>
        IObjectSet<T> Execute<T>(bool inMemory) where T : class;


        /// <summary>
        /// executes the Query.
        /// </summary>
        /// <param name="inMemory">Indicates if all returned data should be loaded to memory (inMemory is true)
        /// or if the data should be lazy loaded (inMemory to false)</param>
        /// <param name="startIndex">Start index for result page.</param>
        /// <param name="endIndex">End index for result page.</param>
        /// <returns>ObjectSet - the result of the Query.</returns>
        IObjectSet<T> Execute<T>(bool inMemory, int startIndex, int endIndex) where T : class;


        /// <summary>
        /// returns a reference to a descendant node in the query graph.
        /// If the node does not exist, it will be created.
        /// 
        /// All classes represented in the query node are tested, whether
        /// they contain a field with the specified field name. The
        /// descendant Query node will be created from all possible candidate
        /// classes.
        /// </summary>
        /// <param name="attributeName">field path to the descendant.</param>
        /// <returns>descendant Query node</returns>
        IQuery Descend(string attributeName);

        
        /// <summary>
        /// adds an ascending ordering criteria to this node of
        /// the query graph. Multiple ordering criteria will be applied
        /// in the order they were called.
        /// @return this Query object to allow the chaining of method calls.
        /// </summary>
        IQuery OrderAscending();


        /// <summary>
        /// adds a descending order criteria to this node of
        /// the query graph. Multiple ordering criteria will be applied
        /// in the order they were called.
        /// @return this Query object to allow the chaining of method calls.
        /// </summary>
        IQuery OrderDescending();


        /// <summary>
        ///   Returns true if the query has an order by clause
        /// </summary>
        /// <returns> true if has an order by flag </returns>
        bool HasOrderBy();

        /// <summary>
        ///   Returns the field names of the order by
        /// </summary>
        /// <returns> The array of fields of the order by </returns>
        IList<string> GetOrderByFieldNames();

        /// <returns> the type of the order by - ORDER_BY_NONE,ORDER_BY_DESC,ORDER_BY_ASC </returns>
        OrderByConstants GetOrderByType();

        /// <summary>
        ///   used with isForSingleOid == true, to indicate we are working on a single object with a specific oid
        /// </summary>
        /// <returns> </returns>
        OID GetOidOfObjectToQuery();

        Type UnderlyingType { get; }

        long Count();
    }
}
