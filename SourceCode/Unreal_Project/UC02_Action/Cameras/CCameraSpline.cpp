#include "CCameraSpline.h"
#include "Global.h"
#include "Components/SplineComponent.h"

ACCameraSpline::ACCameraSpline()
{
	CHelpers::CreateComponent(this, &Spline, "Spline");
	CHelpers::GetAsset<UCurveFloat>(&CurveFloat, "CurveFloat'/Game/Cameras/Curve_Camera.Curve_Camera'");
}

