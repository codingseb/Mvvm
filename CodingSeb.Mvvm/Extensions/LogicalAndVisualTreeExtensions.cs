using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace System.Windows
{
    public static class LogicalAndVisualTreeExtensions
    {
        public static FrameworkElement GetTopFrameworkElement(this FrameworkElement element)
        {
            return !(element.Parent is FrameworkElement) ? element : (element.Parent as FrameworkElement)?.GetTopFrameworkElement();
        }

        /// <summary>
        /// To find a parent (Ancestor) of the specified type in the visual tree
        /// Begin from the current child and go up until it find the parent or reach the root
        /// </summary>
        /// <typeparam name="T">The type of the parent we search for</typeparam>
        /// <param name="child">A DependencyObject child from where to start the search</param>
        /// <param name="ancestorLevel">The level of ancestor of the specified Type to return by default : 1</param>
        /// <returns>The first found parent of the specified type, or null if none are found</returns>
        public static T FindVisualParent<T>(this DependencyObject child, int ancestorLevel = 1)
            where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (parentObject is T parent)
            {
                if (ancestorLevel > 1)
                    ancestorLevel--;
                else
                    return parent;
            }

            // use recursion to proceed with next level
            return FindVisualParent<T>(parentObject, ancestorLevel);
        }

        /// <summary>
        /// To find a parent(Ancestor) of the specified type in the visual tree
        /// Begin from the current child and go up until it find the parent or reach the root
        /// </summary>
        /// <param name="child">A DependencyObject child from where to start the search</param>
        /// <param name="typeOfParent">The type of the parent we search for</param>
        /// <param name="ancestorLevel">The level of ancestor of the specified Type to return by default : 1</param>
        /// <returns>The first found parent of the specified type, or null if none are found</returns>
        public static DependencyObject FindVisualParent(this DependencyObject child, Type typeOfParent, int ancestorLevel = 1)
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (typeOfParent.IsAssignableFrom(parentObject.GetType()))
            {
                if (ancestorLevel > 1)
                    ancestorLevel--;
                else
                    return parentObject;
            }

            // use recursion to proceed with next level
            return FindVisualParent(parentObject, typeOfParent, ancestorLevel);
        }

        /// <summary>
        /// To find a parent (Ancestor) of the specified type in the logical tree
        /// Begin from the current child and go up until it find the parent or reach the root
        /// </summary>
        /// <typeparam name="T">The type of the parent we search for</typeparam>
        /// <param name="child">A DependencyObject child from where to start the search</param>
        /// <param name="ancestorLevel">The level of ancestor of the specified Type to return by default : 1</param>
        /// <returns>The first found parent of the specified type, or null if none are found</returns>
        public static T FindLogicalParent<T>(this DependencyObject child, int ancestorLevel = 1)
            where T : DependencyObject
        {
            // get parent item
            var parentObject = LogicalTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (parentObject is T parent)
            {
                if (ancestorLevel > 1)
                    ancestorLevel--;
                else
                    return parent;
            }

            // use recursion to proceed with next level
            return FindLogicalParent<T>(parentObject, ancestorLevel);
        }

        /// <summary>
        /// To find a parent (Ancestor) of the specified type in the logical tree
        /// Begin from the current child and go up until it find the parent or reach the root
        /// </summary>
        /// <param name="child">A DependencyObject child from where to start the search</param>
        /// <param name="typeOfParent">The type of the parent we search for</param>
        /// <param name="ancestorLevel">The level of ancestor of the specified Type to return by default : 1</param>
        /// <returns>The first found parent of the specified type, or null if none are found</returns>
        public static DependencyObject FindLogicalParent(this DependencyObject child, Type typeOfParent, int ancestorLevel = 1)
        {
            // get parent item
            var parentObject = LogicalTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (typeOfParent.IsAssignableFrom(parentObject.GetType()))
            {
                if (ancestorLevel > 1)
                    ancestorLevel--;
                else
                    return parentObject;
            }

            // use recursion to proceed with next level
            return FindLogicalParent(parentObject, typeOfParent, ancestorLevel);
        }

        /// <summary>
        /// Search in the Visual tree for a descendant (child) of the specified type.
        /// Begin from the current parent and scan children
        /// </summary>
        /// <typeparam name="T">The type of the child we search for</typeparam>
        /// <param name="parent">The current element to search for child</param>
        /// <returns>The first descendant that is from the specified type</returns>
        public static T VisualFirstDescendantOfType<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var result = (child as T) ?? VisualFirstDescendantOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Search in the Logical tree for a child of the specified type.
        /// Begin from the current parent and scan children
        /// </summary>
        /// <typeparam name="T">The type of the child we search for</typeparam>
        /// <param name="parent">The current element to search for child</param>
        /// <returns>The first descendant that is from the specified type</returns>
        public static T LogicalFirstDescendantOfType<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null) return null;

            foreach(var child in LogicalTreeHelper.GetChildren(parent))
            {
                var result = (child as T) ?? LogicalFirstDescendantOfType<T>(child as DependencyObject);
                if (result != null) return result;
            }
            return null;
        }

        public static object FindNearestResource(this DependencyObject self, object resourceKey)
        {
            FrameworkElement frameworkElement = self as FrameworkElement ?? self.FindLogicalParent<FrameworkElement>();
            object resource = frameworkElement?.Resources[resourceKey];

            while (frameworkElement != null && resource == null)
            {
                frameworkElement = frameworkElement.FindLogicalParent<FrameworkElement>();
                resource = frameworkElement?.Resources[resourceKey];
            }

            if (resource == null)
            {
                resource = Application.Current.Resources[resourceKey];
            }

            return resource;
        }

        /// <summary>
        /// Get an enumerable of all sub elements in logical tree of the current element
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<object> LogicalTreeDepthFirstTraversal(this DependencyObject node)
        {
            if (node == null) yield break;
            yield return node;

            foreach (var child in LogicalTreeHelper.GetChildren(node).OfType<DependencyObject>()
                .SelectMany(LogicalTreeDepthFirstTraversal))
            {
                yield return child;
            }
        }

        /// <summary>
        /// Get an enumerable of all sub elements in visual tree of the current element
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<object> VisualTreeDepthFirstTraversal(this DependencyObject node)
        {
            if (node == null) yield break;
            yield return node;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
            {
                var child = VisualTreeHelper.GetChild(node, i);
                foreach (var d in child.VisualTreeDepthFirstTraversal())
                {
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Return an enumerable of the visual ancestory of the current DependencyObject (including the starting point).
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> VisualTreeAncestory(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));

            return VisualTreeAncestory2();

            IEnumerable<DependencyObject> VisualTreeAncestory2()
            {
                while (dependencyObject != null)
                {
                    yield return dependencyObject;
                    dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                }
            }
        }

        /// <summary>
        /// Return an enumerable of the logical ancestory of the current DependencyObject (including the starting point).
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> LogicalTreeAncestory(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));

            return LogicalTreeAncestory2();

            IEnumerable<DependencyObject> LogicalTreeAncestory2()
            {
                while (dependencyObject != null)
                {
                    yield return dependencyObject;
                    dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
                }
            }
        }
    }
}
