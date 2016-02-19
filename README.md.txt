The avatar in the game is controlled by pre-defined 7 commands which are currently embedded in the code. 
The SenseInputRecognition script is used to configure the voice commands from the player. And in the future, this could be made configurable by the player.

RealSense Download Link
-----------------------
https://software.intel.com/en-us/intel-realsense-sdk?gclid=CjwKEAiA9JW2BRDxtaq2ruDg22oSJACgtTxc3u2FV4aDa7hmc_qg9RRVoBrysjFNnMjQt-QwVu_YNhoCZ87w_wcB&gclsrc=aw.ds


Script Information
------------------
Assets\Standard Assets\Scripts\SenseInputRecognition.cs - Intel RealSense SDK bridge script.
Assets\Standard Assets\Scripts\SpeechRecognizer.cs - Converts voice recognized words to match actions in Third person controller script.
Assets\Standard Assets\Characters\ThirdPersonCharacter\Scripts\ThirdPersonUserControl.cs - Controls the avatar's movement based on the voice commands.


Commands and Action
-------------------
“Forward” - Moves the player in the forward direction.
“Back” - Moves the player in the backward direction.
“Left” - Moves the player in the Left direction.
“Right” - Moves the player in the Right direction.
“Stay” - Stops the player movement.
“Duck” – Player moves to crouching position.
“Up” – Player retrieves form crouching position.
