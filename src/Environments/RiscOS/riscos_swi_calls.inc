/*
// https://www.riscosopen.org/wiki/documentation/show/OS%20SWI%20Calls

case 0x00: break; // OS_WriteC
case 0x01: break; // OS_WriteS
*/
[[reko::service(vector=0x02)]]
[[reko::returns(register,"r0")]] 
char *
OS_Write0(
	[[reko::arg(register,"r0")]] char * text);

[[reko::service(vector=0x03)]]
void 
OS_NewLine();

[[reko::service(vector=0x04)]]
[[reko::returns(flag, "C")]]
bool OS_ReadC(
	[[reko::arg(out,register,"r0")]] char c);

/*
case 0x05: break; // OS_CLI
*/
[[reko::service(vector=0x06)]]
[[reko::returns(register, "r0")]]
unsigned char
OS_Byte(
	[[reko::arg(register, "r0")]] int reason_code,
	[[reko::arg(register, "r1")]] unsigned int new_value,
	[[reko::arg(register, "r2")]] unsigned int mask);

/*
case 0x07: break; // OS_Word
case 0x08: break; // OS_File
case 0x09: break; // OS_Args
case 0x0A: break; // OS_BGet
case 0x0B: break; // OS_BPut
case 0x0C: break; // OS_GBPB
case 0x0D: break; // OS_Find
case 0x0E: break; // OS_ReadLine
case 0x0F: break; // OS_Control
case 0x10: break; // OS_GetEnv
*/
[[reko::service(vector=0x11)]]
[[noreturn]] 
void
OS_Exit(
	[[reko::arg(register,"r0")]] void * error_block,
	[[reko::arg(register,"r1")]] char * abex,
	[[reko::arg(register,"r2")]] int return_code);

[[reko::service(vector=0x12)]]
void OS_SetEnv(
	[[reko::arg(register,"r0")]] void * exitHandler,				// Address of the handler for OS_Exit, 0 to read
	[[reko::arg(register,"r1")]] void * memLimit,					// Address of the memory limit for OS_GetEnv, 0 to read
	[[reko::arg(register,"r4")]] void * undefinedInstrHandler,		// Address for undefined instructions, 0 to read
	[[reko::arg(register,"r5")]] void * prefetchAbortHandler,		// Address for prefetch abort, 0 to read
	[[reko::arg(register,"r6")]] void * dataAbortHandler,			// Address for handler for data abort, 0 to read
	[[reko::arg(register,"r7")]] void * addressExceptionHandler,    // Address for handler for address exceptions, 0 to read 

	[[reko::arg(out,register,"r0")]] void * exitHandlerOut,				// Address of the handler for OS_Exit, 0 to read
	[[reko::arg(out,register,"r1")]] void * memLimitOut,					// Address of the memory limit for OS_GetEnv, 0 to read
	[[reko::arg(out,register,"r4")]] void * undefinedInstrHandlerOut,		// Address for undefined instructions, 0 to read
	[[reko::arg(out,register,"r5")]] void * prefetchAbortHandlerOut,		// Address for prefetch abort, 0 to read
	[[reko::arg(out,register,"r6")]] void * dataAbortHandlerOut,			// Address for handler for data abort, 0 to read
	[[reko::arg(out,register,"r7")]] void * addressExceptionHandlerOut    // Address for handler for address exceptions, 0 to read 
	);
/*
case 0x13: break; // OS_IntOn
case 0x14: break; // OS_IntOff
case 0x15: break; // OS_CallBack
case 0x16: break; // OS_EnterOS
case 0x17: break; // OS_BreakPt
case 0x18: break; // OS_BreakCtrl
case 0x19: break; // OS_UnusedSWI
case 0x1A: break; // OS_UpdateMEMC
case 0x1B: break; // OS_SetCallBack
case 0x1C: break; // OS_Mouse
case 0x1D: break; // OS_Heap
case 0x1E: break; // OS_Module
case 0x1F: break; // OS_Claim
case 0x20: break; // OS_Release
case 0x21: break; // OS_ReadUnsigned
case 0x22: break; // OS_GenerateEvent
case 0x23: break; // OS_ReadVarVal
case 0x24: break; // OS_SetVarVal
case 0x25: break; // OS_GSInit
case 0x26: break; // OS_GSRead
case 0x27: break; // OS_GSTrans
case 0x28: break; // OS_BinaryToDecimal
case 0x29: break; // OS_FSControl
case 0x2A: break; // OS_ChangeDynamicArea
*/
[[reko::service(vector=0x2B)]]
[[reko::returns(register,"r0")]]
unsigned int OS_GenerateError([[reko::arg(register,"r0")]] void * error_block);
/*
case 0x2C: break; // OS_ReadEscapeState
case 0x2D: break; // OS_EvaluateExpression
case 0x2E: break; // OS_SpriteOp
case 0x2F: break; // OS_ReadPalette
case 0x30: break; // OS_ServiceCall
case 0x31: break; // OS_ReadVduVariables
case 0x32: break; // OS_ReadPoint
case 0x33: break; // OS_UpCall
case 0x34: break; // OS_CallAVector
case 0x35: break; // OS_ReadModeVariable
case 0x36: break; // OS_RemoveCursors
case 0x37: break; // OS_RestoreCursors
case 0x38: break; // OS_SWINumberToString
case 0x39: break; // OS_SWINumberFromString
case 0x3A: break; // OS_ValidateAddress
case 0x3B: break; // OS_CallAfter
case 0x3C: break; // OS_CallEvery
case 0x3D: break; // OS_RemoveTickerEvent
case 0x3E: break; // OS_InstallKeyHandler
case 0x3F: break; // OS_CheckModeValid
case 0x40: break; // OS_ChangeEnvironment
case 0x41: break; // OS_ClaimScreenMemory
case 0x42: break; // OS_ReadMonotonicTime
case 0x43: break; // OS_SubstituteArgs
case 0x44: break; // OS_PrettyPrint
case 0x45: break; // OS_Plot
case 0x46: break; // OS_WriteN
case 0x47: break; // OS_AddToVector
case 0x48: break; // OS_WriteEnv
case 0x49: break; // OS_ReadArgs
case 0x4A: break; // OS_ReadRAMFsLimits
case 0x4B: break; // OS_ClaimDeviceVector
case 0x4C: break; // OS_ReleaseDeviceVector
case 0x4D: break; // OS_DelinkApplication
case 0x4E: break; // OS_RelinkApplication
case 0x4F: break; // OS_HeapSort
case 0x50: break; // OS_ExitAndDie
case 0x51: break; // OS_ReadMemMapInfo
case 0x52: break; // OS_ReadMemMapEntries
case 0x53: break; // OS_SetMemMapEntries
case 0x54: break; // OS_AddCallBack
case 0x55: break; // OS_ReadDefaultHandler
case 0x56: break; // OS_SetECFOrigin
case 0x57: break; // OS_SerialOp
case 0x58: break; // OS_ReadSysInfo
case 0x59: break; // OS_Confirm
case 0x5A: break; // OS_ChangedBox
case 0x5B: break; // OS_CRC
case 0x5C: break; // OS_ReadDynamicArea
case 0x5D: break; // OS_PrintChar
case 0x5E: break; // OS_ChangeRedirection
case 0x5F: break; // OS_RemoveCallBack
case 0x60: break; // OS_FindMemMapEntries
case 0x61: break; // OS_SetColour
case 0x64: break; // OS_Pointer
case 0x65: break; // OS_ScreenMode
case 0x66: break; // OS_DynamicArea
case 0x68: break; // OS_Memory
case 0x69: break; // OS_ClaimProcessorVector
case 0x6A: break; // OS_Reset
case 0x6B: break; // OS_MMUControl
case 0x6C: break; // OS_ResyncTime
case 0x6D: break; // OS_PlatformFeatures
case 0x6E: break; // OS_SynchroniseCodeAreas
case 0x6F: break; // OS_CallASWI
case 0x70: break; // OS_AMBControl
case 0x71: break; // OS_CallASWIR12
case 0x72: break; // OS_SpecialControl?
case 0x73: break; // OS_EnterUSR32
case 0x74: break; // OS_EnterUSR26
case 0x75: break; // OS_VIDCDivider?
case 0x76: break; // OS_NVMemory
case 0x7A: break; // OS_Hardware
case 0x7B: break; // OS_IICOp
case 0x7C: break; // OS_LeaveOS
case 0x7D: break; // OS_ReadLine32
case 0x7E: break; // OS_SubstituteArgs32
case 0x7F: break; // OS_HeapSort32
case 0xC0: break; // OS_ConvertStandardDateAndTime
case 0xC1: break; // OS_ConvertDateAndTime
case 0xD0: break; // OS_ConvertHex1
case 0xD1: break; // OS_ConvertHex2
case 0xD2: break; // OS_ConvertHex4
case 0xD3: break; // OS_ConvertHex6
case 0xD4: break; // OS_ConvertHex8
case 0xD5: break; // OS_ConvertCardinal1
case 0xD6: break; // OS_ConvertCardinal2
case 0xD7: break; // OS_ConvertCardinal3
case 0xD8: break; // OS_ConvertCardinal4
case 0xD9: break; // OS_ConvertInteger1
case 0xDA: break; // OS_ConvertInteger2
case 0xDB: break; // OS_ConvertInteger3
case 0xDC: break; // OS_ConvertInteger4
case 0xDD: break; // OS_ConvertBinary1
case 0xDE: break; // OS_ConvertBinary2
case 0xDF: break; // OS_ConvertBinary3
case 0xE0: break; // OS_ConvertBinary4
case 0xE1: break; // OS_ConvertSpacedCardinal1
case 0xE2: break; // OS_ConvertSpacedCardinal2
case 0xE3: break; // OS_ConvertSpacedCardinal3
case 0xE4: break; // OS_ConvertSpacedCardinal4
case 0xE5: break; // OS_ConvertSpacedInteger1
case 0xE6: break; // OS_ConvertSpacedInteger2
case 0xE7: break; // OS_ConvertSpacedInteger3
case 0xE8: break; // OS_ConvertSpacedInteger4
case 0xE9: break; // OS_ConvertFixedNetStation
case 0xEA: break; // OS_ConvertNetStation
case 0xEB: break; // OS_ConvertFixedFileSize
case 0xEC: break; // OS_ConvertFileSize
case 0x100..0x1FF: break; // OS_WriteI
*/