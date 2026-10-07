using Edf.Application.Operator.WorkContinuity;
using Edf.Domain.Projects;

namespace Edf.Application.Tests.Operator;

public class OperatorWorkFocusSubjectProposalTests
{
    [Fact]
    public void RetainedMvr0005ProjectId_EstablishesGovernedVerificationReference()
    {
        var projectId = new ProjectConcordProjectId(OperatorGovernedVerificationReferences.Mvr0005RetainedProjectId);

        var found = OperatorWorkFocusSubjectProposal.TryFromGovernedVerificationContext(
            projectId,
            "any display name",
            out var key,
            out var label);

        Assert.True(found);
        Assert.Equal(OperatorGovernedVerificationReferences.Mvr0005Key, key);
        Assert.Equal(OperatorGovernedVerificationReferences.Mvr0005DefaultSubjectLabel, label);
    }

    [Fact]
    public void DisplayNameContainingMvr0005_DoesNotEstablishGovernedReference()
    {
        var projectId = ProjectConcordProjectId.New();

        var found = OperatorWorkFocusSubjectProposal.TryFromGovernedVerificationContext(
            projectId,
            "My folder MVR-0005 experiment",
            out var key,
            out var label);

        Assert.False(found);
        Assert.Empty(key);
        Assert.Empty(label);
    }
}
