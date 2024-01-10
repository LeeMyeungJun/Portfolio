#include "CFileRW.h"
#include "Global.h"
#include "Serialization/BufferArchive.h"

ACFileRW::ACFileRW()
{
	CHelpers::CreateActorComponent<UInputComponent>(this, &InputComponent, "InputComponent");
}

void ACFileRW::BeginPlay()
{
	Super::BeginPlay();

	APlayerController* controller = GetWorld()->GetFirstPlayerController();
	EnableInput(controller);

	InputComponent->BindAction("Save", EInputEvent::IE_Pressed, this, &ACFileRW::OnSave);
	InputComponent->BindAction("Load", EInputEvent::IE_Pressed, this, &ACFileRW::OnLoad);
}

void ACFileRW::OnSave()
{
	FSaveData data;
	data.ID = "someddk";
	data.Health = 100.0f;

	FBufferArchive buffer;
	buffer << data;

	CheckTrue(buffer.Num() < 1);

	buffer.FlushCache();
	FString str = FPaths::ProjectDir() + FString("Test.bin");
	bool b = FFileHelper::SaveArrayToFile(buffer, *str);
	buffer.Empty();

	CLog::Print(b ? "Saved" : "Save Failed");
}

void ACFileRW::OnLoad()
{
	FString str = FPaths::ProjectDir() + FString("Test.bin");
	
	FBufferArchive buffer;
	FFileHelper::LoadFileToArray(buffer, *str);

	FMemoryReader reader = FMemoryReader(buffer, true);
	reader.Seek(0);

	FSaveData data;

	reader << data;

	CLog::Log(data.ID.ToString());
	CLog::Log(data.Health);

	reader.FlushCache();
	reader.Close();

	
}

