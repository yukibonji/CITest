namespace XLCatlin.DataLab.XCBRA.Command

/// Core types used for handling commands

open System
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore

type Command<'id, 'data> = {
    Id : 'id
    Data : 'data
    User : string
    // etc
    }
    
// ====================================   
// Functions that a domain must export for event sourcing
// ====================================
type CommandExecutionError = CommandExecutionError of string

/// Convert a recorded event (e.g. serialized as Json) into a domain event
type FromRecordedEvent<'DomainEvent> = RecordedEvent -> 'DomainEvent

/// Convert a domain event into a writable event (e.g. serialized as Json)
type ToWriteEvent<'DomainEvent> = 'DomainEvent -> WriteEvent

/// Apply a domain event to the current state
type ApplyDomainEvent<'DomainEvent,'State> = 'State option -> 'DomainEvent -> 'State option 

/// Execute a command and return a list of domain events, or a CommandError.
/// The command itself should already be baked in (eg. using partial application)
type ExecuteCommand<'DomainEvent,'State> = 'State option -> Result<'DomainEvent list,CommandExecutionError>

/// Update the read model with the current state
type UpdateReadModel<'State> = 'State -> Result<unit,ReadModelError>

/// Functions that a domain must export for event sourcing
type EventSourcingFunctions<'DomainEvent,'State> = {
    FromRecordedEvent: FromRecordedEvent<'DomainEvent> 
    ToWriteEvent: ToWriteEvent<'DomainEvent> 
    ApplyDomainEvent: ApplyDomainEvent<'DomainEvent,'State> 
    ExecuteCommand: ExecuteCommand<'DomainEvent,'State> 
    UpdateReadModel: UpdateReadModel<'State> 
    DeleteReadModel: UpdateReadModel<'State> 
    }

/// Functions that a domain must export for replaying events
type ReplayEventFunctions<'DomainEvent,'State> = {
    FromRecordedEvent: FromRecordedEvent<'DomainEvent> 
    ApplyDomainEvent: ApplyDomainEvent<'DomainEvent,'State> 
    UpdateReadModel: UpdateReadModel<'State> 
    }