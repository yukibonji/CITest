namespace XLCatlin.DataLab.XCBRA.DomainModel

// -- Domain type --------------------------------------------------------------

type WarehouseUserInput = 
    { // -- Building ------------------
      activityType : UserResponse<UncertainObservation<List<Activity * Percentage>>>
    ; floorArea  : UserResponse<RangeEstimate<Area>>
    ; yearBuilt : UserResponse<Year>
    ; buildingHeight : UserResponse<UncertainObservation<float<Metre>>>
    ; buildingValue : UserResponse<AssetValue>
    ; roofCombustibility : UserResponse<UncertainObservation<Combustibility>>
    ; wallsCombustibility : UserResponse<UncertainObservation<Combustibility>>

    // -- Sprinklers -----------------------------------------------------------
    ; sprinklerDesignCode : UserResponse<UncertainObservation<DesignCode>>
    ; sprinklerAdequacy : UserResponse<UncertainObservation<bool>>
    ; sprinklersInstallExpertise : UserResponse<UncertainObservation<Percentage>>
    ; sprinklerLikelihoodOfImpairment : UserResponse<UncertainObservation<Percentage>>
    ; sprinklersPercentFloorAreaCovered : UserResponse<UncertainObservation<Percentage>>
    ; automaticFireDetection : UserResponse<UncertainObservation<List<QualitativeRating>>>


    // -- Stock ----------------------------------------------------------------
    ; stockHeight : UserResponse<UncertainObservation<float<Metre>>>
    ; stockDensity : UserResponse<UncertainObservation<Percentage>>
    ; stockSkidded : UserResponse<UncertainObservation<Percentage>>
    ; stockClassification : UserResponse<UncertainObservation<List<StockGroup * Percentage>>>
    ; stockPeakValue : UserResponse<AssetValue>
    ; stockNeedsControlledTemp : UserResponse<UncertainObservation<bool>>
    ; stockIsHeatSensitive : UserResponse<UncertainObservation<bool>>
    ; stockIsSmokeSensitive : UserResponse<UncertainObservation<bool>>
    ; stockIsWaterSensitive : UserResponse<UncertainObservation<bool>>


    // -- Fire Brigade ---------------------------------------------------------
    ; fireBrigadeType : UserResponse<UncertainObservation<FireBrigade>>
    ; fireBrigadeDistanceInMins : UserResponse<RangeEstimate<DurationInMins>>
    ; fireFightingEffectiveness : UserResponse<UncertainObservation<Percentage>>
    ; waterSupplyAndHydrantQuality : UserResponse<UncertainObservation<QualitativeRating >>


    // -- Workers --------------------------------------------------------------

    ; workerShifts : UserResponse<UncertainObservation<List<Shifts * Percentage>>>
    ; healthSafetyTraining : UserResponse<UncertainObservation<Percentage>>
    ; workerDensity : UserResponse<UncertainObservation<WorkerDensity>>
    ; workerTurnover : UserResponse<UncertainObservation<WorkerTurnover>>

    // -- Infrastructure -------------------------------------------------------

    ; hasARS : UserResponse<UncertainObservation<bool>>
    ; hasCarousel : UserResponse<UncertainObservation<bool>>
    ; hasHeatingOrAC : UserResponse<UncertainObservation<bool>>
    ; forklifts : UserResponse<UncertainObservation<List<Forklift * Percentage>>>
    ; machineryValue : UserResponse<AssetValue>
    ; lighting : UserResponse<UncertainObservation<List<Lighting * Percentage>>>
    ; infrastructureQuality : UserResponse<UncertainObservation<Percentage>>

    }


//FireSpreadRoofAffected : UserResponse<UncertainObservation<bool>>
//IgnitionLocationOf : UserResponse<UncertainObservation<bool>>


// Client -- TODO: this should live in the client entity...
//ClientCorpRiskEngStandardsYN : UserResponse<UncertainObservation<bool>>
//ClientHighlyDevelopedYN : UserResponse<UncertainObservation<bool>>
//ClientProfessionalRiskManagerYN : UserResponse<UncertainObservation<bool>>
//ClientDomicile : UserResponse<Country>


// Location -- TODO: these should live the location entity
//LocationSiteClimateNonTemperate : UserResponse<UncertainObservation<bool>>
//LocationSiteSpaceConstraintPresent : UserResponse<UncertainObservation<bool>>
//LocationSiteUrbanSuburbanOrRural : UserResponse<UncertainObservation<LocationType>>
//LocationSiteFormalImpairmentMgmtProgram : UserResponse<UncertainObservation<QualitativeRating>>
//LocationSiteGAPSRiskManagementScore : UserResponse<UncertainObservation<QualitativeRating>>
//LocationSiteRiskManagement : UserResponse<UncertainObservation<UnitInterval>>
//SecurityOnSite : UserResponse<UncertainObservation<UnitInterval>>
//BuildingExternalSiteExposureRating : UserResponse<UncertainObservation<BuildingExternalSiteExposure>>



[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>] 
module WarehouseUserInput = 
    let empty = 
        { activityType = NotAsked 
        ; floorArea  = NotAsked 
        ; yearBuilt = NotAsked 
        ; buildingHeight = NotAsked 
        ; buildingValue = NotAsked 
        ; roofCombustibility = NotAsked 
        ; wallsCombustibility = NotAsked 

        ; sprinklerDesignCode = NotAsked 
        ; sprinklerAdequacy = NotAsked 
        ; sprinklersInstallExpertise = NotAsked 
        ; sprinklerLikelihoodOfImpairment = NotAsked 
        ; sprinklersPercentFloorAreaCovered = NotAsked 
        ; automaticFireDetection = NotAsked 


        // Stock 
        ; stockHeight = NotAsked 
        ; stockDensity = NotAsked 
        ; stockSkidded = NotAsked 
        ; stockClassification = NotAsked 
        ; stockPeakValue = NotAsked
        ; stockNeedsControlledTemp = NotAsked 
        ; stockIsHeatSensitive = NotAsked 
        ; stockIsSmokeSensitive = NotAsked 
        ; stockIsWaterSensitive = NotAsked 


        // -- Fire Brigade ---------------------------------------------------------
        ; fireBrigadeType = NotAsked 
        ; fireBrigadeDistanceInMins = NotAsked 
        ; fireFightingEffectiveness = NotAsked 
        ; waterSupplyAndHydrantQuality = NotAsked 


        // -- Workers --------------------------------------------------------------

        ; workerShifts = NotAsked 
        ; healthSafetyTraining = NotAsked 
        ; workerDensity = NotAsked 
        ; workerTurnover = NotAsked 

        // -- Infrastructure -------------------------------------------------------

        ; hasARS = NotAsked 
        ; hasCarousel = NotAsked 
        ; hasHeatingOrAC = NotAsked 
        ; forklifts = NotAsked 
        ; machineryValue = NotAsked 
        ; lighting = NotAsked 
        ; infrastructureQuality = NotAsked 

        }


type Warehouse = Building<WarehouseUserInput>
    
// -- Commands -----------------------------------------------------------------

type WarehouseCommand =
    | Create of buildingId : BuildingId * locationId : LocationId * name: string
    | Delete of buildingId : BuildingId 
    | UpdateName of buildingId : BuildingId * name : string

    | AnswerActivityType of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Activity * Percentage>>>
    | AnswerFloorArea  of buildingId : BuildingId * response :  UserResponse<RangeEstimate<Area>>
    | AnswerYearBuilt of buildingId : BuildingId * response :  UserResponse<Year>
    | AnswerBuildingHeight of buildingId : BuildingId * response :  UserResponse<UncertainObservation<float<Metre>>>
    | AnswerBuildingValue of buildingId : BuildingId * response :  UserResponse<AssetValue>
    | AnswerRoofCombustibility of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Combustibility>>
    | AnswerWallsCombustibility of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Combustibility>>

    | AnswerSprinklerDesignCode of buildingId : BuildingId * response :  UserResponse<UncertainObservation<DesignCode>>
    | AnswerSprinklerAdequacy of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerSprinklersInstallExpertise of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerSprinklerLikelihoodOfImpairment of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerSprinklersPercentFloorAreaCovered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerAutomaticFireDetection of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<QualitativeRating>>>

    | AnswerStockHeight of buildingId : BuildingId * response :  UserResponse<UncertainObservation<float<Metre>>>
    | AnswerStockDensity of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerStockSkidded of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerStockClassification of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<StockGroup * Percentage>>>
    | AnswerStockPeakValue of buildingId : BuildingId * response :  UserResponse<AssetValue>
    | AnswerStockNeedsControlledTemp of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerStockIsHeatSensitive of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerStockIsSmokeSensitive of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerStockIsWaterSensitive of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>

    | AnswerFireBrigadeType of buildingId : BuildingId * response :  UserResponse<UncertainObservation<FireBrigade>>
    | AnswerFireBrigadeDistanceInMins of buildingId : BuildingId * response :  UserResponse<RangeEstimate<DurationInMins>>
    | AnswerFireFightingEffectiveness of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerWaterSupplyAndHydrantQuality of buildingId : BuildingId * response :  UserResponse<UncertainObservation<QualitativeRating >>

    | AnswerWorkerShifts of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Shifts * Percentage>>>
    | AnswerHealthSafetyTraining of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AnswerWorkerDensity of buildingId : BuildingId * response :  UserResponse<UncertainObservation<WorkerDensity>>
    | AnswerWorkerTurnover of buildingId : BuildingId * response :  UserResponse<UncertainObservation<WorkerTurnover>>

    | AnswerHasARS of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerHasCarousel of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerHasHeatingOrAC of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | AnswerForklifts of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Forklift * Percentage>>>
    | AnswerMachineryValue of buildingId : BuildingId * response :  UserResponse<AssetValue>
    | AnswerLighting of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Lighting * Percentage>>>
    | AnswerInfrastructureQuality of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>





// -- Events -------------------------------------------------------------------

type WarehouseEvent =
    | Created of buildingId : BuildingId * locationId : LocationId * name: string
    | Deleted of buildingId : BuildingId 
    | NameUpdated of buildingId : BuildingId * name : string
    | ActivityTypeAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Activity * Percentage>>>
    | FloorAreaAnswered  of buildingId : BuildingId * response :  UserResponse<RangeEstimate<Area>>
    | YearBuiltAnswered of buildingId : BuildingId * response :  UserResponse<Year>
    | BuildingHeightAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<float<Metre>>>
    | BuildingValueAnswered of buildingId : BuildingId * response :  UserResponse<AssetValue>
    | RoofCombustibilityAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Combustibility>>
    | WallsCombustibilityAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Combustibility>>

    | SprinklerDesignCodeAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<DesignCode>>
    | SprinklerAdequacyAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | SprinklersInstallExpertiseAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | SprinklerLikelihoodOfImpairmentAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | SprinklersPercentFloorAreaCoveredAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | AutomaticFireDetectionAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<QualitativeRating>>>

    | StockHeightAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<float<Metre>>>
    | StockDensityAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | StockSkiddedAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | StockClassificationAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<StockGroup * Percentage>>>
    | StockPeakValueAnswered of buildingId : BuildingId * response :  UserResponse<AssetValue>
    | StockNeedsControlledTempAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | StockIsHeatSensitiveAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | StockIsSmokeSensitiveAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | StockIsWaterSensitiveAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>

    | FireBrigadeTypeAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<FireBrigade>>
    | FireBrigadeDistanceInMinsAnswered of buildingId : BuildingId * response :  UserResponse<RangeEstimate<DurationInMins>>
    | FireFightingEffectivenessAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | WaterSupplyAndHydrantQualityAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<QualitativeRating >>

    | WorkerShiftsAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Shifts * Percentage>>>
    | HealthSafetyTrainingAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>
    | WorkerDensityAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<WorkerDensity>>
    | WorkerTurnoverAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<WorkerTurnover>>

    | HasARSAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | HasCarouselAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | HasHeatingOrACAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<bool>>
    | ForkliftsAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Forklift * Percentage>>>
    | MachineryValueAnswered of buildingId : BuildingId * response :  UserResponse<AssetValue>
    | LightingAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<List<Lighting * Percentage>>>
    | InfrastructureQualityAnswered of buildingId : BuildingId * response :  UserResponse<UncertainObservation<Percentage>>

