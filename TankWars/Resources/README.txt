CREATORS:

Tyler Amy (u0790600)
Lindsey Loveland (u0970740)

POLISH:
	We made it so when a tank is hit by a projectile, the health bar decreases and changes color according to how many times it was hit
	We added an 'explosion' that triggers whenever a tank has been killed

WHAT WORKS AND WHAT DOESN'T:
	Everything required in PS8 works in our program as intended. 

OUR PROCESS --
	The Handshake:
		Set up communication and connection with the server using our NetworkController class
		Handle the data sent by the server, create world according to the provided size
		Handle the data received by state.GetData()
	Creating the Form:
		First initialize the form, create the world according the the size given by the server
			This is where we added the World class containing our tanks, projectiles, and powerups	
		Add labels, textboxes, and connect button to the form
		Once connect is clicked, connect to the server address that was provided and use World Constructor to create the world
		Create and initialize form and world on "connectButton_click" event
	Parsing and Handling Server Data
		In ReceiveWorld, we created a string arrray called parts in which we separated the data given to us by the server by \n
		Iterate through parts, check if the obj on the current line is a tank, proj, beam, power, or wall
		Remove the data from the state after we add it to the world
		Call UpdateArrived() which notifies the View that a redraw of the frame is required
	Drawing (Our OnPaint Method)
		In our onPaint method, we iterate through all our dictionaries in our World Class, and use our DrawObjectWithTransform method to draw each
		individual part
		Create the separate drawers for wall, tank, projectiles, etcetera. Ended up having to create a separate "UnderTankDrawer" for our health bar and player info
	Movement
		We registered events to our mouseClicks, mouseMovements, and KeyPresses that communicate with the server accordingly via ControlCommand
	Closing
		Handled the closing of the game and server by referencing the MVC Example in the Examples Repository