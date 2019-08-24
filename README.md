
# Discord Walker
Inspired by [Discord Invite Finder](https://github.com/Jaminima/DiscordInviteFinder) which used a Brute-Force method of finding invites,</br>
But with some major improvements in terms of speed and efficiency
## How does it work?
To function we use a Discord User account, to achieve this we make use of the [Discord User API](https://github.com/Jaminima/DiscordUserAPI) library,</br>
Which makes it nice and easy to imitate being a user.

To find invites we first start by joining the Invite Code provided by the user,</br>
We then search through the first 100 messages in each text channel and add the found invite links to a queue,</br>
Upon completion of the search of the guild, we repeat these steps</br>
Using the Invite Link at the start of the queue.

Given a good starting guild, we should be able to search indefinitely.
## Issues
Due to the nature of what the User account is doing, it is quickly flagged</br>
And banned by discord, hence a delay is used to try and extend the life of the account.</br>
There is currently no known way to avoid this, bar excessively large delays.
## Getting Started
To get started, add your details into Accounts.txt in /Debug/Data, an example is available [here](https://github.com/Jaminima/DiscordWalker/ExampleData/Accounts.txt)</br>
Modify this line `List<String> Codes  =  Backend.Walker.StartWalking(GetInstances(),"", 10, 180);`</br>
In Program.cs, change the `""` to include the starting invite code, eg: `"RK7HVE"` NOT `"https://discord.gg/RK7HVE"`</br>
The following 2 paramaters define, firstly how many discords to Walk through and secondly how many seconds between joining each discord.