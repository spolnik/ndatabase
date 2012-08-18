using System;

namespace NDatabase.Odb.Core.Query.Soda
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
        /// 
        /// Special behaviour for:
        /// <ul>
        /// <li> class Class: confine the result to objects of one
        /// class (if the Class} object represents a class)
        /// or to objects implementing a specific interface
        /// (if the Class object represents an interface).</li>
        /// <li> interface Evaluation: run
        /// evaluation callbacks against all candidates.</li>
        /// </ul>
        /// @param constraint the constraint to be added to this Query.
        /// @return Constraint a new Constraint for this
        /// query node or <code>null</code> for objects implementing the 
        /// Evaluation interface.
        /// </summary>
        IConstraint Constrain(Object constraint);


        /// <summary>
        /// returns a Constraints
        /// object that holds an array of all constraints on this node.
        /// @return Constraints on this query node.
        /// </summary>
        IConstraints Constraints();


        /// <summary>
        /// executes the Query.
        /// @return ObjectSet - the result of the Query}.
        /// </summary>
        IObjectSet Execute();


        /// <summary>
        /// returns a reference to a descendant node in the query graph.
        /// If the node does not exist, it will be created.
        /// 
        /// All classes represented in the query node are tested, whether
        /// they contain a field with the specified field name. The
        /// descendant Query node will be created from all possible candidate
        /// classes.
        /// @param field path to the descendant.
        /// @return descendant Query node
        /// </summary>
        IQuery Descend(String fieldName);


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
    }
}
