namespace Tests;

public class ChainTests
{
	[Theory]
	[InlineData( 0.0f,  0.0f,  0.0f)] // add nothing
	[InlineData( 1.0f,  0.5f,  1.5f)] // add
	[InlineData( 1.0f, -0.5f,  0.5f)] // subtract result positive
	[InlineData( 0.5f, -1.0f, -0.5f)] // subtract result regative
	[InlineData(-1.0f, -0.5f, -1.5f)] // subtract both
	[InlineData(-1.0f,  1.0f,  0.0f)] // cancel out
	public void CalculateAbsoluteAngleLimit_ReturnsExpectedValue(float aboveRotation, float relativeLimit, float expected)
	{
		float result = Chain.CalculateAbsoluteAngleLimit(aboveRotation, relativeLimit);
		
		Assert.Equal(expected, result, 5); // to 5 decimal places
	}
	
	[Theory]
	[InlineData(1.0f, 1.0f, 2,  true)] // correct
	[InlineData(1.0f, 2.0f, 2, false)] // incorrect
	[InlineData(1.235f, 1.239f, 2,  true)] // correct round up
	[InlineData(1.234f, 1.231f, 2,  true)] // correct round down
	[InlineData(1.234f, 1.239f, 2, false)] // incorrect round up
	[InlineData(1.235f, 1.231f, 2, false)] // incorrect round down
	[InlineData(-1.234f, -1.231f, 2,  true)] // correct negative
	[InlineData(-1.234f, -1.239f, 2, false)] // incorrect negative
	[InlineData(1.0f, 0.111111111111f, 2, false)] // big decimal
	public void EvaluateNumsRounded_ReturnsExpectedValue(float num1, float num2, int decimalPlaces, bool expected)
	{
		bool result = Chain.EvaluateNumsRounded(num1, num2, decimalPlaces);
		
		Assert.Equal(expected, result);
	}
	
	[Theory]
	[InlineData( 0.0f, -1.0f, 1.0f, false)] // inside
	[InlineData(-1.0f, -1.0f, 1.0f,  true)] // exactly lower
	[InlineData( 1.0f, -1.0f, 1.0f,  true)] // exactly upper
	[InlineData(-2.0f, -1.0f, 1.0f,  true)] // outside lower
	[InlineData( 2.0f, -1.0f, 1.0f,  true)] // outside upper
	[InlineData(-0.999f, -1.0f, 1.0f,  false)] // inside lower decimal
	[InlineData( 0.999f, -1.0f, 1.0f,  false)] // inside upper decimal
	public void IsRotationOutsideLimits_ReturnsExpectedValue(float rotation, float lowerLimit, float upperLimit, bool expected)
	{
		bool result = Chain.IsRotationOutsideLimits(rotation, lowerLimit, upperLimit);
		
		Assert.Equal(expected, result);
	}
	
	[Theory]
	[InlineData(0.0f,  10.0f, 2.0f, 0.1f, 0.0f,  0.05f)] 
	[InlineData(1.0f,  10.0f, 2.0f, 0.1f, 0.0f,  0.15f)] 
	[InlineData(0.0f, -10.0f, 2.0f, 0.1f, 0.0f, -0.05f)] 
	[InlineData(2.0f,  0.0f,  2.0f, 0.1f, 1.0f,  1.20f)] 
	public void CalculateRotationNextPhysicsTick_ReturnsExpectedValue(float angularVelocity, float torque, float inertia, float delta, float rotation, float expected)
	{
		float result = Chain.CalculateRotationNextPhysicsTick(angularVelocity, torque, inertia, delta, rotation);
		
		Assert.Equal(expected, result, 5);
	}
}
