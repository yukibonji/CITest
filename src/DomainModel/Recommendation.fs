namespace XLCatlin.DataLab.XCBRA.DomainModel

// Issue- and recommendation-related

// ==========================================
// Domain types
// ==========================================

type IssueType = 
    | A
    | B
    | C

type Issue =  { 
    Description : string
    IssueType : IssueType
    }

type Resolution =
    | FollowUp
    | Closed

type Recommendation = {
    Issue : Issue
    Resolution : Resolution
    }

