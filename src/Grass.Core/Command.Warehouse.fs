namespace XLCatlin.DataLab.XCBRA.Command

(*
Event-sourcing code for Warehouse aggregates

* Commands and Events are defined in the DomainModel project
* Functions that are specific to this aggregate:
    * applyEvent - given a event and a current state, generate a new state
    * executeCommand - given a command and a precommand state, generate a list of domain events
    * toWriteEvent - converts a domain event into a standard WriteEvent that can be serialized
    * fromRecordedEvent - converts a commited RecordedEvent into a domain event
*)

open System
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore

module Warehouse =

    // ==========================
    // Command handling and event application
    // ==========================

    let create buildingId locationId name  : Warehouse = 
        { id = buildingId
        ; locationId = locationId 
        ; name  = name
        ; occupancy = Occupancy.Warehouse 
        ; userInput = WarehouseUserInput.empty
        }

    /// Apply one event to the (optional) state and return the new (optional) state
    let applyEvent (stateOpt:Warehouse option) event =
        match stateOpt, event with
        | None , WarehouseEvent.Created(buildingId, locationId, name) ->
            Some <| 
                create buildingId locationId name

        | Some _ , WarehouseEvent.Deleted _  -> 
                None 

        | Some state, WarehouseEvent.NameUpdated(_, name) ->
            Some
                { state with
                    name =  name
                }

        | Some state , WarehouseEvent.ActivityTypeAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with activityType = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.FloorAreaAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with floorArea = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.YearBuiltAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with yearBuilt = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.BuildingHeightAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with buildingHeight = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.BuildingValueAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with buildingValue = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.RoofCombustibilityAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with roofCombustibility = response }

            Some    
                { state with
                    userInput = newUserInput
                }


        | Some state , WarehouseEvent.WallsCombustibilityAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with wallsCombustibility = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.SprinklerDesignCodeAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with sprinklerDesignCode = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.SprinklerAdequacyAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with sprinklerAdequacy = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.SprinklersInstallExpertiseAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with sprinklersInstallExpertise = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.SprinklerLikelihoodOfImpairmentAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with sprinklerLikelihoodOfImpairment = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.SprinklersPercentFloorAreaCoveredAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with sprinklersPercentFloorAreaCovered = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.AutomaticFireDetectionAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with automaticFireDetection = response }

            Some    
                { state with
                    userInput = newUserInput
                }


        | Some state , WarehouseEvent.StockHeightAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockHeight = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockDensityAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockDensity = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockSkiddedAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockSkidded = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockClassificationAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockClassification = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockPeakValueAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockPeakValue = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockNeedsControlledTempAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockNeedsControlledTemp = response }

            Some    
                { state with
                    userInput = newUserInput
                }


        | Some state , WarehouseEvent.StockIsHeatSensitiveAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockIsHeatSensitive = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockIsSmokeSensitiveAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockIsSmokeSensitive = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.StockIsWaterSensitiveAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with stockIsWaterSensitive = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.FireBrigadeTypeAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with fireBrigadeType = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.FireBrigadeDistanceInMinsAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with fireBrigadeDistanceInMins = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.FireFightingEffectivenessAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with fireFightingEffectiveness = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.WaterSupplyAndHydrantQualityAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with waterSupplyAndHydrantQuality = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.WorkerShiftsAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with workerShifts = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.HealthSafetyTrainingAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with healthSafetyTraining = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.WorkerDensityAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with workerDensity = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.WorkerTurnoverAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with workerTurnover = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.HasARSAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with hasARS = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.HasCarouselAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with hasCarousel = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.HasHeatingOrACAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with hasHeatingOrAC = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.ForkliftsAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with forklifts = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.MachineryValueAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with machineryValue = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.LightingAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with lighting = response }

            Some    
                { state with
                    userInput = newUserInput
                }

        | Some state , WarehouseEvent.InfrastructureQualityAnswered(_, response) ->
            let userInput =  state.userInput
            let newUserInput =  { userInput with infrastructureQuality = response }

            Some    
                { state with
                    userInput = newUserInput
                }

                                
        | _,_ ->
            let details = sprintf "%A %A" stateOpt event 
            failwithf "Unexpected combination of event and state: %s" details 


    let buildingIdFromCommand cmd =
        match cmd with
        | WarehouseCommand.Create(buildingId , _ , _) -> 
            buildingId

        | WarehouseCommand.Delete(buildingId) ->
            buildingId 

        | WarehouseCommand.UpdateName(buildingId,_) ->
            buildingId 
        
        | WarehouseCommand.AnswerActivityType(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerFloorArea(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerYearBuilt(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerBuildingHeight(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerBuildingValue(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerRoofCombustibility(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerWallsCombustibility(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerSprinklerDesignCode(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerSprinklerAdequacy(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerSprinklersInstallExpertise(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerSprinklerLikelihoodOfImpairment(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerSprinklersPercentFloorAreaCovered(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerAutomaticFireDetection(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockHeight(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockDensity(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockSkidded(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockClassification(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockPeakValue(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockNeedsControlledTemp(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockIsHeatSensitive(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockIsSmokeSensitive(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerStockIsWaterSensitive(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerFireBrigadeType(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerFireBrigadeDistanceInMins(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerFireFightingEffectiveness(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerWaterSupplyAndHydrantQuality(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerWorkerShifts(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerHealthSafetyTraining(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerWorkerDensity(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerWorkerTurnover(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerHasARS(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerHasCarousel(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerHasHeatingOrAC(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerForklifts(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerMachineryValue(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerLighting(buildingId,_) ->
            buildingId

        | WarehouseCommand.AnswerInfrastructureQuality(buildingId,_) ->
            buildingId


    /// Execute the command against the (optional) precommand state and 
    /// return the events arising, or Result.Error if the command was invalid.
    let executeCommand cmd precommandStateOpt = 
        let buildingId = 
            buildingIdFromCommand cmd

        match precommandStateOpt,cmd with
        | None, WarehouseCommand.Create(buildingId,locationId,name) ->
            Ok 
                [WarehouseEvent.Created(buildingId,locationId,name)
                ]

        | Some _ , WarehouseCommand.Create _ ->
            Error << CommandExecutionError <|
                sprintf "Can't create. %A already exists" buildingId 


        | None, _ ->
            Error << CommandExecutionError <|
                sprintf "%A has not been created" buildingId 
            

        | _ , WarehouseCommand.Delete(buildingId) ->
            Ok 
                [ WarehouseEvent.Deleted(buildingId)
                ]

        | _ , WarehouseCommand.UpdateName(buildingId,name) ->
            Ok 
                [ WarehouseEvent.NameUpdated(buildingId,name)
                ]

        | _ , WarehouseCommand.AnswerActivityType(buildingId,response) ->
            Ok 
                [ WarehouseEvent.ActivityTypeAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerFloorArea(buildingId,response) ->
            Ok 
                [ WarehouseEvent.FloorAreaAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerYearBuilt(buildingId,response) ->
            Ok 
                [ WarehouseEvent.YearBuiltAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerBuildingHeight(buildingId,response) ->
            Ok 
                [ WarehouseEvent.BuildingHeightAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerBuildingValue(buildingId,response) ->
            Ok 
                [ WarehouseEvent.BuildingValueAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerRoofCombustibility(buildingId,response) ->
            Ok 
                [ WarehouseEvent.RoofCombustibilityAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerWallsCombustibility(buildingId,response) ->
            Ok 
                [ WarehouseEvent.WallsCombustibilityAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerSprinklerDesignCode(buildingId,response) ->
            Ok 
                [ WarehouseEvent.SprinklerDesignCodeAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerSprinklerAdequacy(buildingId,response) ->
            Ok 
                [ WarehouseEvent.SprinklerAdequacyAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerSprinklersInstallExpertise(buildingId,response) ->
            Ok 
                [ WarehouseEvent.SprinklersInstallExpertiseAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerSprinklerLikelihoodOfImpairment(buildingId,response) ->
            Ok 
                [ WarehouseEvent.SprinklerLikelihoodOfImpairmentAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerSprinklersPercentFloorAreaCovered(buildingId,response) ->
            Ok 
                [ WarehouseEvent.SprinklersPercentFloorAreaCoveredAnswered(buildingId,response)
                ]
        | _ , WarehouseCommand.AnswerAutomaticFireDetection(buildingId,response) ->
            Ok 
                [ WarehouseEvent.AutomaticFireDetectionAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockHeight(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockHeightAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockDensity(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockDensityAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockSkidded(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockSkiddedAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockClassification(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockClassificationAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockPeakValue(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockPeakValueAnswered(buildingId,response)
                ]
        | _ , WarehouseCommand.AnswerStockNeedsControlledTemp(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockNeedsControlledTempAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockIsHeatSensitive(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockIsHeatSensitiveAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockIsWaterSensitive(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockIsWaterSensitiveAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerStockIsSmokeSensitive(buildingId,response) ->
            Ok 
                [ WarehouseEvent.StockIsSmokeSensitiveAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerFireBrigadeType(buildingId,response) ->
            Ok 
                [ WarehouseEvent.FireBrigadeTypeAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerFireBrigadeDistanceInMins(buildingId,response) ->
            Ok 
                [ WarehouseEvent.FireBrigadeDistanceInMinsAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerFireFightingEffectiveness(buildingId,response) ->
            Ok 
                [ WarehouseEvent.FireFightingEffectivenessAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerWaterSupplyAndHydrantQuality(buildingId,response) ->
            Ok 
                [ WarehouseEvent.WaterSupplyAndHydrantQualityAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerWorkerShifts(buildingId,response) ->
            Ok 
                [ WarehouseEvent.WorkerShiftsAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerHealthSafetyTraining(buildingId,response) ->
            Ok 
                [ WarehouseEvent.HealthSafetyTrainingAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerWorkerDensity(buildingId,response) ->
            Ok 
                [ WarehouseEvent.WorkerDensityAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerWorkerTurnover(buildingId,response) ->
            Ok 
                [ WarehouseEvent.WorkerTurnoverAnswered(buildingId,response)
                ]                  
            
        | _ , WarehouseCommand.AnswerHasARS(buildingId,response) ->
            Ok 
                [ WarehouseEvent.HasARSAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerHasCarousel(buildingId,response) ->
            Ok 
                [ WarehouseEvent.HasCarouselAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerHasHeatingOrAC(buildingId,response) ->
            Ok 
                [ WarehouseEvent.HasHeatingOrACAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerForklifts(buildingId,response) ->
            Ok 
                [ WarehouseEvent.ForkliftsAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerMachineryValue(buildingId,response) ->
            Ok 
                [ WarehouseEvent.MachineryValueAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerLighting(buildingId,response) ->
            Ok 
                [ WarehouseEvent.LightingAnswered(buildingId,response)
                ]

        | _ , WarehouseCommand.AnswerInfrastructureQuality(buildingId,response) ->
            Ok 
                [ WarehouseEvent.InfrastructureQualityAnswered(buildingId,response)
                ]     


    // A literal so we can use it in pattern matching
    [<Literal>]
    let EventStreamPrefix = "WarehouseId:"

    /// Get the stream id for a location
    let idToEventStream (BuildingId id) =
        sprintf "%s%s" EventStreamPrefix id 
        |> EventStreamId

    let eventStreamIdFromCommand cmd = 
        let buildingId = buildingIdFromCommand cmd
        buildingId |> idToEventStream 

    /// Given the serialized data, create a LocationEvent
    let fromRecordedEvent : FromRecordedEvent<WarehouseEvent> =
        fun recordedEvent -> 
            //TODO: consider using a format that supports versioning
            Microsoft.FSharpLu.Json.Compact.deserialize<WarehouseEvent>(recordedEvent.Data)    
            // exception handling is done in the CommandService

    /// Given a LocationEvent, create a WriteEvent 
    let toWriteEvent : ToWriteEvent<WarehouseEvent> =
        fun domainEvent -> 
            let data = Microsoft.FSharpLu.Json.Compact.serialize(domainEvent)    
            {
            EventId = Guid.NewGuid()
            EventType = "WarehouseEvent"  // could store info to help the deserializer
            Data = data
            Metadata = ""
            }

    /// Update the entity in the read model (on disk on in cache)
    let updateReadModel (readModelStore:IReadModelStore) (state:Warehouse) = 
        readModelStore.UpdateWarehouse state

    /// Delete the entity from the read model (on disk on in cache)
    let deleteReadModel (readModelStore:IReadModelStore) (state:Warehouse) = 
        readModelStore.DeleteWarehouse state.id

    let eventSourcingFunctions cmd readModelStore = {
        FromRecordedEvent = fromRecordedEvent
        ToWriteEvent = toWriteEvent
        ApplyDomainEvent = applyEvent
        ExecuteCommand = executeCommand cmd 
        UpdateReadModel = updateReadModel readModelStore
        DeleteReadModel = deleteReadModel readModelStore
        }

    let replayEventFunctions readModelStore = {
        FromRecordedEvent = fromRecordedEvent
        ApplyDomainEvent = applyEvent
        UpdateReadModel = updateReadModel readModelStore
        }