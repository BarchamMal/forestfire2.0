# Forest Fire 2.0
This is my second attempt at forest fire simulations, the first (and 0th, and -1th) attempt(s) are not on github as they are slow and clunky and junky. This simulation is a C# Avalonia project, with a python script for some data visualization. I'm not certain if it would run on windows, of course, but it certainly runs on mac.

## The Simulation
The simulation appears as a pixelated image of colors, black, brown, and shades of green. As you watch, oranges and yellows will appear.

The simulation consists of a number of cells, a grid of them in fact, by default, 200x200, with each cell being 8 pixels wide. The behavior of the sim is, in short, that dirt grows into trees, which can be struck by lightning and burnt down.

The behavior of the simulation, in long, is that there are several cell types:
- Charred
- Dirt
- Grass
- Brush
- Tree
- Thick Tree
- Fire

And each one can turn into at least one other under some conditions, these I will explain:
1. Charred cells, a blackish color, can randomly change into dirt cells.
2. Dirt cells, a brownish color, can randomly change into grass cells.
3. Grass cells, a light green color, can randomly change into brush cells, or can catch fire (with a low change) from nearby fires, and turn into fire cells.
4. Brush cells, a neutral green color, can randomly change into tree cells, or can catch fire (with a somewhat higher chance) from nearby fires, and turn into fire cells.
5. Tree cells, a leafy green color, can randomly change into Thick Tree cells, or can catch fire from nearby fires, or be struck by lightning, and turn into fire cells.
6. Thick tree cells, a deep green color, can catch fire from nearby fires, or be struck by lightning (twice as often as normal trees) and turn into fire cells.
7. Fire cells, a yellow or orangeish color, will, shortly after being created, transform into charred cells.
This, playing out, has the effect of black areas turning brown, then light and slowly deeper green, simulating plant growth, then being burnt down to black again by pale yellow orange fires.
It is also worthy of note, that because, in real life, forest growth is on a year and decade timescale, and wildfire growth is on a day and month timescale, I have made the sim to pause except for fire grown during any wildfires.

## The Python script
The python script is for those who are less interested in the visual and more in the mathmatical aspects of the simulation.
While the simulation runs, it slowly, or not slowly, fills up a file, called `fire-sizes.txt` with all the wildfires ever in the simulation. This file is written to, every one hundred wildfires, and the format is simple; every line contains a number which is the size of a fire that happened. Due to the fact that the file is written to so periodically, one could perhaps run many instances of the simulation in parallel to gather data faster.

The Python script, `analytics.py` is made to analyze this file, and display the information in several ways, all of which I will explain:
1. `-concat`: this is the first command you're likely to use, as it reads the fire-sizes file, and outputs a new file, called c-sizes. c-sized contains essentially the same data, but not as a fire-per-line format, instead it lists fire sizes and their frequencies.
2. `-graph`: is also a useful command. It opens up a graph, graphing the frequencies of the fires. You should expect to see a power curve, with a massive head and tail.
3. `-p2g`: is much less useful, but still interesting. It opens up a graph, showing the frequency of fires in power of two bins, how many of size 1, 2 and 3, 4-8, 9-16, 17-32, 33-64, and so on. It is expected to look like (with smooth data, mine was 800k data points) a smooth graphu nera the top until you reach the middle of the graph on the x axis where it dips down to almost nothing and stays like that.
4. `-log`: is also interesting, more so than `-p2g`. This command, or flag, creates a log log graph of the data, which tends to look like a straight donw-rightward slop with a hairy tail, which fades as data points grow.
5. `-bin` or `-b`: is a flag used to bin data in `-graph` or `-log`. It is used with a number after it, being the size of each data bin. Though it smoothes out the data when data points are few, it also skews the data in odd ways, so be wary using it.

All the "commands" I've mentioned are argparse flags, and the script can be run as, for example:
`python3 analytics.py -log -b 5`

Enjoy the simulation and script, if you find any bugs, feel free to either make an [issue](https://github.com/BarchamMal/forestfire2.0/issues/choose) or fork the repo, fix it yourself and make a pull request.
