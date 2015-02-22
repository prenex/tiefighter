# tiefighter
An open source XNA 4.0 Game called TIE Fighter Forever, inspired by the original TIE Fighter game.

Please read the LICENCE file, before reading this so that you do not stole any ideas from me :-)

I've decided to release this work of mine to the public as it seems I rarery touch it now but people might get interested... Historical note: I'm only doing this now as a girl friend of mine asked me to do some I-never-did-it little changes to the code so that the demo game becomes more enjoyable, but I got the idea of "oh... why not make this open to public?" so read on ;-)

GENARALLY:
------------------
This is a game written in C# using the XNA 4.0 framework for my BSc thesis theme. I had big plans with this game, like adding a big tactical map somewhat resembling a part of the "real" star wars universe (see the Essential Atlas maps for example) where you can build your fleet of capital and smaller ships and make tactical moves in a wing commander armada-like fashion.

The battles would be after that automatically generated: The ships and they position/orientation, the old TIE-Fighter-like primary and secondary objectives, and even issued commands towards you and othere ships by the strategist AI that you should go to that point in space shooting down all missles in your cubic space you are in (effectively shielding a capital ship against torpedoes) etc.

So the main idea was that in start of a battle scene, the objectives would be generated, but there are also these changing little quests for which you will see a yellow arrow about current targets if you accept an arising "quest" (in the above case you will see a yellow arrow towards the cubic space around the ship to defend, if you comes there, automatically you get targets for incoming torpedoes).

Also I had a storyline:
- The game starts after the second death star gets destroyed (with Palpatine too) and thus having a great confusion in the Empire.
- Your main character will be "Priest". You should know... that is the cloacked figure in the old TIE Fighter game that always assigned you secondary objectives for getting into the inner circle of the emperor. It would turn out, he is a man who descended from the "prophets of the dark side" order (they are not Siths) and never really liked the emperor.
-After the chaos and collapse, he finds a system that is hard to navigate in and basically there are only one well-known navigational trade route into the sector and thus it is a place that is pretty much easy to control if you control the navigation point of getting into it. I was thinking about the "centrality" area with junkfort station as the entry point and having (the already lifeless) old dark-side planet of Tund as the main lair for Priest (he would build an orbiting station there). Also the system will have some small sienar factories you want to aquire and some small planets.
- First ou seemingly only want to liberate the old ruling class of planets in centrality and your plan is to not let this area rebel against the empire with the usual rebels, but on its own. Because of this you should wage war against the incoming rebel "terrorists", the local remnants of "bad old empire" and of course pirates and likes. This seems hard with the small fleets of the centrality, however as the empire is in chaos, the rebels also usually fight with the remnants of the empire and pirates are no that much threat that you cannot overcome, Priest build its own place to rule, like a new smaller empire that is also ruled by the dark side, just not leaded by a sith, but the possibly last darkside prophet who makes excurions to the dead Tund to find old lores to became stronger.

More about "Centrality":  http://starwars.wikia.com/wiki/Centrality

There was an idea that you could go with several kind of characters in battle-mode:
1.) You could pilot unnamed pilots and kind of switch to an other already engaded fighter when you die. You can choose the craft you want before start or after you die one craft.
2.) You could pilot with Priest: in this case you will have a special dark-prophet skill that is like an extra hud and extra quest interface. Game should generate quests for disabling ships to board, finding special cargo and so on. Also you might get some battle benefits by providing the player much more information than usual.

Other ideas:
Different starships would not only get different weapons like in other games, but even they should be differently controlled, should have different targeting computers and semantically different hud. So the hud is not only different by having this and that at another places, but for example a TIE hud can have the standard tie/xwing radar half spheres in the top corners that show what is behind you and before you, while some centrality ships might have a radar on the center of the bottom of the hud like in ELITE games (just start elite or oolite to see what I'm talking about). Also some primitive ships might even have only 2 dimensional radar where you cannot know how much above is an object.
- I was always curious about how TIE fighters can be sometimes better than rebel fighters so I had the idea in my mind that I make TIE-fighters like ships for linux hackers and pro players: they would be fragile and spartan and even hard to steer correctly in battle however they will be great if you have skill in them. So in a software parallel they would be the linux, vi, perl, regex and stuff. Also I guess you can have better strategist AI command interface so you get precise commands for defending spaceships, attacking interesting targets etc. Strategically they would be like the Zergs of starcraft :-)
- On the other hand I kind of think that rebel starfighters must be much better as a single-person manouvering and should be equipped more like something like a bigger ship. They would get much better targeting computers, of course shields and warheads etc. also They would be more easy to steer as I planned a freespace-like control method for them so you could take out TIEs so easily if the TIE pilot is not a very skilled bastard. :-)
- There should be exceptions for the above rules... like A-wings can have the "pro" control method and assoult gunboats can have the "freespace" kind of.... I think you get the idea...
- Also I have the idea that the player could control also ships that are neither capital ships, nor fighters. Shuttles, excorts shuttles and transports (maybe even corellian or assault transports! :)) have been planned to also have the freespace-like control - just in a delayed and slow-turning way... - and the turbo-lasers that your crew members operate will constantly fire for the nearby enemies. These ships are like slow big tanks on todays battlefields and can store many torpedoes, a landing party or such, however because of the lumbering small move they do, they will be easy to take out by fighters, as usual and also fighter-bombers can be better in some situations.


FROM THE ABOVE THINGS, NEARLY NOTHING IS DONE, HOWEVER:
- I had the idea of beautifully glowing laserfire. We do have it!
- We have normal mapping, environment mapping, particle system for explosions....
- We have a nicely done collision engine: There are collision classes like big ships, small ships and bullets. Small ships collide with small ships with fast collisions by thinking about the ship as a sphere, small ships collide with big ones by having a pre-compiled octree data structure and per-triangle collision (it is pretty fast as it only computes the fighter's collision against the triangles of the big ship in octree nodes the fighter is in - this even works if the big ship moves or changes orientation as it is all done translated to that coordinate system.
- Also there are methods for bullets hitting big and small ships. The bullets are entities on they own so you can friendly fire easily.
- big ships never collide with big ships!
+ TODO: I've planned to include a dynamic octree for collisions between fighters and between lasers and fighters. I think I can implement this and after that there can be no 50s-100s of fighters but maybe thousands!
+ The collision engine really lies many times and unreal, however I wanted speed there not precisity. For the given cases the models are "precise enough" for having fun while playing. Even old TIE and XWING was not physically correct never, but they anyways had the simulation feeling so I wanted to have the same for my game ;-)

- The game was developed by having X-BOX in mind, however I do not know how much of this is lost. I think it can be made compatible even now.
- Also the game not only scales well, but in the current demo state it runs well on my laptop where I did started to develop this back in 2008-2009: An ASUS with a single-core (!) 1.6Ghz processor, having an 256Mb video card that uses my system ram, 896MB of system ram (I think even 512Mb is enough!) and has shader model 2.0 in it. Now it generally and practically should run on all possible machines however I think the effects are still not that bad. If you start working on this, I ask you to please introduce changes only if the sysreq does not get much higher as otherwise I could not play it anymore :-)

- There are interfaces for AI, menus and stuff.
- Also because this was a thesis work, the code has documentation comments and also there is many technical details in the (Hungarian) thesis document of mine. I will put it into the repository however I think only few people could read Hungarian (anyways it's a pretty nice language really mystic and loreful, so you could check it out). Maybe once I will translate at least the table of contents and will answer to emails if there are any questions....

FOR LINUX USERS:
- When I wrote this game I was not a big fan of linux, and I've planned to release this by maybe selling it if it is finally completed so this is in C# and XNA4...... Also XNA was a nice thing for me as before this game I've only did 3D programming in assembly (Masm32) and opengl immediate mode, but this game introduced me to shaders and some then-modern stuff. The content pipeline helped much to create big data structures for collision octrees of starships and model loading and etc. etc. while I was able to create a scene management and other things easily concentrating on real work. Also there were pre-existing classes for every matrix and vector/quaternion transformation and that helps as I was in a really hurry while writing this code...
- Anyways I'm now became a really linux-guy, who like suckess.org, dwm, FOSS and integrates vim plugins into every editor so I had tought about booting this game repository up as a monogame project but I'm really confused about monogame state of the art... I kind of fear that it still misses too many content pipeline stuff that I would need and also I have little time to work on this game as a hobbie so do not expect me to make this change soon. IF YOU CAN MANAGE TO START THIS UP IN MONOGAME, I WILL CHANGE TO THAT!!! I mean it!!! It would be huge to have a game like this on linux, while practically running on every machine and I might work on it more I think ;-)
