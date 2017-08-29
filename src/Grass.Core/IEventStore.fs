namespace XLCatlin.DataLab.XCBRA.EventStore

(*
Types and interfaces for the EventStore
*)

open System
open XLCatlin.DataLab.XCBRA.DomainModel

/// Events are associated with a particular "event stream"
/// When reading or saving events, the stream must be specified.
/// Typically, there is one stream for each Aggregate, as well as 
/// other streams for other events.
/// We will assume that all event streams are identified by a string.
/// Example:
///  * A particular location -- "LocationId:A"
///  * Summary of all locations -- "LocationSummary"
///
/// IMPORTANT: Each event stream must be complete independent from the others.
type EventStreamId = XLCatlin.DataLab.XCBRA.DomainModel.EventStreamId 

/// A stream has a monotonically increasing version which is changed
/// every time the stream is modified.
/// When saving an event, an expected stream version must be provided,
/// and if the version does match current version of the stream,
/// a VersionConflict error is returned.
type EventStreamVersion = XLCatlin.DataLab.XCBRA.DomainModel.EventStreamVersion

/// Passed in to Write operation for concurrency
/// See http://docs.geteventstore.com/dotnet-api/3.0.1/optimistic-concurrency-and-idempotence/
type ExpectedVersion = XLCatlin.DataLab.XCBRA.DomainModel.ExpectedVersion


/// An event to write to the store.
/// Note that the actual content of the event is in "Data".
/// How the content is serialized to this field is a separate concern, outside the EventStore.
type WriteEvent = {
    EventId : Guid
    EventType : string
    Data : string
    Metadata : string
    }

/// An event as read from the store.
/// Note that the actual content of the event is in "Data".
/// How the content is deserialized from this field is a separate concern, outside the EventStore.
type RecordedEvent= {
    StreamId : EventStreamId
    EventId : Guid
    EventNumber : int
    EventType : string
    Data : string
    Metadata : string
    Created : DateTime
    }


/// Returned on a successful commit
type EventStreamWriteResult = {
    StreamId: EventStreamId
    StreamVersion : EventStreamVersion
    Timestamp: DateTime
    RecordedEvents : RecordedEvent list
    }

/// Returned on a successful read
type EventStreamReadResult = {
    StreamId: EventStreamId
    StreamVersion : EventStreamVersion
    Events: RecordedEvent list
    }


/// Errors that are not part of the domain
type EventStreamInfrastructureError = 
    | AuthenticationError of string * exn
    /// Returned if the network times out or there are other problems
    | ExceptionError of string * exn


/// The list of things that go wrong in an event stream when reading
type EventStreamReadError =
    /// When reading a stream, the stream must exist
    | StreamNotFound of string
    | InfrastructureError of EventStreamInfrastructureError

/// Returned on a conflict 
type EventStreamConflict = {
    StreamId: EventStreamId
    ExpectedStreamVersion : EventStreamVersion
    }

/// The list of things that go wrong in an event stream when writing
type EventStreamWriteError =
    /// When saving to a stream, the stream must have the expected version id.
    /// If the stream has been modified and the version has changed, then the update will fail.
    | VersionConflict of EventStreamConflict
    | InfrastructureError of EventStreamInfrastructureError



type IEventStore = 

    /// Save the provided event to a specified event stream, and return a "EventStreamCommit"
    /// containing the new strean version, etc.
    /// If the stream doesnt exist, create it and ignore the passed in version.
    /// If a version is passed the versions don't match, return an error.
    abstract member AppendEventsToStream : 
        EventStreamId -> 
        expectedVersion:ExpectedVersion -> 
        WriteEvent list -> 
        Result<EventStreamWriteResult,EventStreamWriteError>

    /// Read all the events from a specified event stream. Events are returned oldest first.
    abstract member ReadEventsForwards : 
        EventStreamId -> 
        Result<EventStreamReadResult,EventStreamReadError>

    /// List all event streams. 
    /// Used when rebuilding the read model from scratch.
    abstract member AllEventStreamIds : 
        unit ->
        Result<EventStreamId list,EventStreamReadError>

