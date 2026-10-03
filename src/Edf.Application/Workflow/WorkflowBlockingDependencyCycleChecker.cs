using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

internal static class WorkflowBlockingDependencyCycleChecker
{
    /// <summary>
    /// Returns true when adding pending edge (blocked -> required) would create a cycle in the pending precedence graph.
    /// Precedence: required must complete before blocked (edge required -> blocked).
    /// Cycle when blocked can already reach required through pending edges (adding required→blocked would close a loop).
    /// </summary>
    public static bool WouldCreateCycle(
        WorkflowInstanceId blocked,
        WorkflowInstanceId required,
        IReadOnlyList<WorkflowDependency> pendingDependencies)
    {
        var adjacency = BuildPrecedenceAdjacency(pendingDependencies);
        return CanReach(blocked, required, adjacency);
    }

    private static Dictionary<WorkflowInstanceId, List<WorkflowInstanceId>> BuildPrecedenceAdjacency(
        IReadOnlyList<WorkflowDependency> pendingDependencies)
    {
        var map = new Dictionary<WorkflowInstanceId, List<WorkflowInstanceId>>();
        foreach (var dep in pendingDependencies)
        {
            if (dep.Status != WorkflowDependencyStatus.Pending)
            {
                continue;
            }

            if (!map.TryGetValue(dep.RequiredWorkflowInstanceId, out var list))
            {
                list = new List<WorkflowInstanceId>();
                map[dep.RequiredWorkflowInstanceId] = list;
            }

            list.Add(dep.BlockedWorkflowInstanceId);
        }

        return map;
    }

    private static bool CanReach(
        WorkflowInstanceId start,
        WorkflowInstanceId target,
        Dictionary<WorkflowInstanceId, List<WorkflowInstanceId>> adjacency)
    {
        var visited = new HashSet<WorkflowInstanceId>();
        var stack = new Stack<WorkflowInstanceId>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current))
            {
                continue;
            }

            if (current == target)
            {
                return true;
            }

            if (!adjacency.TryGetValue(current, out var next))
            {
                continue;
            }

            foreach (var n in next)
            {
                stack.Push(n);
            }
        }

        return false;
    }
}
