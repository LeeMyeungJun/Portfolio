
#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CLodActor.generated.h"

UCLASS()
class UC02_ACTION_API ACLodActor : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(VisibleDefaultsOnly)
		class UStaticMeshComponent* Mesh;
	
public:	
	ACLodActor();

protected:
	virtual void BeginPlay() override;


private:
	UFUNCTION()
		void PaintVertices();

};
