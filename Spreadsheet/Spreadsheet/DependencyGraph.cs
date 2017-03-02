// Skeleton implementation written by Joe Zachary for CS 3500, January 2017.

using System;
using System.Collections.Generic;

namespace SS
{
	/// <summary>
	/// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
	/// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
	/// s1 equals s2 and t1 equals t2.
	/// 
	/// Given a DependencyGraph DG:
	/// 
	///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
	///    is called the dependents of s, which we will denote as dependents(s).
	///        
	///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
	///    is called the dependees of t, which we will denote as dependees(t).
	///    
	/// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
	///
	/// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
	///     dependents("a") = {"b", "c"}
	///     dependents("b") = {"d"}
	///     dependents("c") = {}
	///     dependents("d") = {"d"}
	///     dependees("a") = {}
	///     dependees("b") = {"a"}
	///     dependees("c") = {"a"}
	///     dependees("d") = {"b", "d"}
	///     
	/// All of the methods below require their string parameters to be non-null.  This means that 
	/// the behavior of the method is undefined when a string parameter is null.  
	///
	/// IMPORTANT IMPLEMENTATION NOTE
	/// 
	/// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
	/// as discussed above.
	/// 
	/// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
	/// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
	/// 
	/// You'll need to be more clever than that.  Design a representation that is both easy to work
	/// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
	/// the test cases with which you will be graded will create massive DependencyGraphs.  If you
	/// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
	/// </summary>
	public class DependencyGraph
	{

		/// <summary>
		/// All of my code should be self explanatory because it is breif other then the use of the deependeeFlag which 
		/// reduces the space complexity of my program by using only a single Dictonary of hash table. It is added to the deependees to mark whck they are.
		///
		/// </summary>

		protected internal Dictionary<string, HashSet<string>> dicDepend;
		private const string dependeeFlag = "!,1qaz2wsx3edc4rfv5tgb6yhn7ujm8ik9o";
		private int dependeeTotal;
		/// <summary>
		/// Creates a DependencyGraph containing no dependencies.
		/// </summary>
		public DependencyGraph()
		{
			dicDepend = new Dictionary<string, HashSet<string>>();

		}
		/// <summary>
		/// Creates a DependencyGraph containing the dependencies
		/// of the provided dg without providing a reference.
		/// </summary>
		public DependencyGraph(DependencyGraph dg)
		{
			var temp = new Dictionary<string, HashSet<string>>(dg.dicDepend);
		}

		/// <summary>
		/// The number of dependencies in the DependencyGraph.
		/// </summary>
		public int Size
		{
			get { return dependeeTotal; }
		}

		/// <summary>
		/// Reports whether dependents(s) is non-empty.  Requires s != null.
		/// </summary>
		public bool HasDependents(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new ArgumentNullException();
			}
			HashSet<string> temp;
			if (dicDepend.TryGetValue(s, out temp))
			{
				return temp.Count != 0;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Reports whether dependees(s) is non-empty.  Requires s != null.
		/// </summary>
		public bool HasDependees(string s)
		{

			s = s + dependeeFlag;
			return HasDependents(s);
		}

		/// <summary>
		/// Enumerates dependents(s).  Requires s != null.
		/// </summary>
		public IEnumerable<string> GetDependents(string s)
		{
			if (string.IsNullOrEmpty(s))
			{

				throw new ArgumentNullException();
			}
			HashSet<string> temp;
			if (dicDepend.TryGetValue(s, out temp))
			{

				return temp;
			}
			else
			{
				temp = new HashSet<string>();
				return temp;
			}


		}

		/// <summary>
		/// Enumerates dependees(s).  Requires s != null.
		/// </summary>
		public IEnumerable<string> GetDependees(string s)
		{
			return GetDependents(s + dependeeFlag);
		}

		/// <summary>
		/// Adds the dependency (s,t) to this DependencyGraph.
		/// This has no effect if (s,t) already belongs to this DependencyGraph.
		/// Requires s != null and t != null.
		/// </summary>
		public void AddDependency(string s, string t)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
			{

				throw new ArgumentNullException();
			}
			HashSet<string> temp;
			if (dicDepend.TryGetValue(s, out temp))
			{
				temp.Add(t);
				dicDepend.Remove(s);
				dicDepend.Add(s, temp);
			}

			else
			{
				temp = new HashSet<string>();
				temp.Add(t);
				dicDepend.Add(s, temp);
			}
			HashSet<string> temp2;
			t = t + dependeeFlag;

			if (dicDepend.TryGetValue(t, out temp2))
			{
				if (!temp2.Contains(s)) { dependeeTotal++; }


				temp2.Add(s);
				dicDepend.Remove(t);
				dicDepend.Add(t, temp2);

			}
			else
			{
				temp2 = new HashSet<string>();
				temp2.Add(s);
				dicDepend.Remove(t);
				dicDepend.Add(t, temp2);
				dependeeTotal++;
			}

		}

		/// <summary>
		/// Removes the dependency (s,t) from this DependencyGraph.
		/// Does nothing if (s,t) doesn't belong to this DependencyGraph.
		/// Requires s != null and t != null.
		/// </summary>
		public void RemoveDependency(string s, string t)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
			{

				throw new ArgumentNullException();
			}
			HashSet<string> temp;
			if (dicDepend.TryGetValue(s, out temp))
			{
				temp.Remove(t);
				dicDepend.Remove(s);
				dicDepend.Add(s, temp);


			}
			HashSet<string> temp2;
			t = t + dependeeFlag;

			if (dicDepend.TryGetValue(t, out temp2))
			{
				if (temp2.Contains(s)) { dependeeTotal--; }
				temp2.Remove(s);
				dicDepend.Remove(t);

				dicDepend.Add(t, temp2);

			}
		}

		/// <summary>
		/// Removes all existing dependencies of the form (s,r).  Then, for each
		/// t in newDependents, adds the dependency (s,t).
		/// Requires s != null and t != null.
		/// </summary>
		public void ReplaceDependents(string s, IEnumerable<string> newDependents)
		{

			if (string.IsNullOrEmpty(s))
			{

				throw new ArgumentNullException();
			}
			var temp = new HashSet<string>(this.GetDependents(s));

			foreach (string tOld in temp)
			{

				this.RemoveDependency(s, tOld);
			}

			foreach (string t in newDependents)
			{
				if (!string.IsNullOrEmpty(t))
				{
					this.AddDependency(s, t);
				}
			}


		}

		/// <summary>
		/// Removes all existing dependencies of the form (r,t).  Then, for each 
		/// s in newDependees, adds the dependency (s,t).
		/// Requires s != null and t != null.
		/// </summary>
		public void ReplaceDependees(string t, IEnumerable<string> newDependees)
		{
			if (string.IsNullOrEmpty(t))
			{
				throw new ArgumentNullException();
			}

			var temp = new HashSet<string>(this.GetDependees(t));

			foreach (string tOld in temp)
			{

				this.RemoveDependency(tOld, t);
			}

			foreach (string s in newDependees)
			{
				if (!string.IsNullOrEmpty(s))
				{
					this.AddDependency(s, t);
				}
			}


		}
	}

}
