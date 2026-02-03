import argparse
from collections import Counter
import matplotlib.pyplot as plt
import math

def load_sizes():
    with open("fire-sizes.txt", "r") as f:
        lines = f.readlines()
    return [int(line.strip()) for line in lines if line.strip()]

def fast_load_sizes():
    with open("c-sizes.txt", "r") as f:
        lines = f.readlines()
    return {int(i.split()[0]): int(i.split()[1]) for i in lines if i.strip()}

def bin_data(counts, bin_size):
    """Counts is a dict {size: frequency}"""
    if bin_size is None:
        return counts

    buckets = {}
    for size, count in counts.items():
        bucket = ((size - 1) // bin_size) * bin_size + 1 if size > 0 else 0
        buckets[bucket] = buckets.get(bucket, 0) + count
    return buckets

def concat():
    sizes = load_sizes()
    counts = Counter(sizes)

    with open("c-sizes.txt", "w") as f:
        for size in sorted(counts.keys()):
            f.write(f"{size} {counts[size]}\n")

def graph(bin_size=None):
    counts = fast_load_sizes()
    counts = bin_data(counts, bin_size)

    x = sorted(counts.keys())
    y = [counts[size] for size in x]

    plt.figure()
    plt.plot(x, y)
    plt.xlabel("Fire Size")
    plt.ylabel("Frequency")
    plt.title("Fire Size Distribution")
    plt.show()

def p2g():
    counts = fast_load_sizes()

    buckets = {}
    for size, count in counts.items():
        if size == 0:
            bucket = 0
        else:
            bucket = int(math.log2(size))
        buckets[bucket] = buckets.get(bucket, 0) + count

    x = sorted(buckets.keys())
    y = [buckets[b] for b in x]

    labels = [f"{2**b}-{2**(b+1)}" for b in x]

    plt.figure()
    plt.plot(x, y, marker='o')
    plt.xticks(x, labels, rotation=45)
    plt.xlabel("Fire Size Range (powers of 2)")
    plt.ylabel("Frequency")
    plt.title("Fire Size Distribution (Powers of 2)")
    plt.tight_layout()
    plt.show()

def log_graph(bin_size=None):
    counts = fast_load_sizes()
    counts = bin_data(counts, bin_size)

    # Filter out size 0 since log(0) is undefined
    x = [s for s in sorted(counts.keys()) if s > 0]
    y = [counts[s] for s in x]

    log_x = [math.log(s) for s in x]
    log_y = [math.log(c) for c in y]

    plt.figure()
    plt.plot(log_x, log_y)
    plt.xlabel("log(Fire Size)")
    plt.ylabel("log(Frequency)")
    plt.title("Log-Log Fire Size Distribution")
    plt.show()

def fireMajority(percent=0):
    """Counts the fires backwards until it reaches percent % of
    the total trees burned, at which time it stops and prints,
    that the fire sizes from here and above represent the total
    percent % of all fires."""
    counts = fast_load_sizes()
    total_burned = sum(size * count for size, count in counts.items())
    target = total_burned * percent

    cumulative = 0
    for size in sorted(counts.keys(), reverse=True):
        count = counts[size]
        cumulative += size * count
        if cumulative >= target:
            print(f"Fires of size {size} and larger represent {percent*100}% of all trees burned,")
            print(f"accounting for {cumulative} out of {total_burned} trees burned.")
            break

def totalBurned(bin_size=None):
    """Creates a graph comaparing the total number of trees burned
    by all fires of the size ever burnt. (y axis) vs the fire size (x axis)."""
    counts = fast_load_sizes()
    counts = bin_data(counts, bin_size)
    x = sorted(counts.keys())
    y = [size * counts[size] for size in x]

    plt.figure()
    plt.plot(x, y)
    plt.xlabel("Fire Size")
    plt.ylabel("Total Trees Burned by Fires of this Size")
    plt.title("Total Trees Burned vs Fire Size")
    plt.show()

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-concat", action="store_true")
    parser.add_argument("-graph", action="store_true")
    parser.add_argument("-p2g", action="store_true")
    parser.add_argument("-log", action="store_true")
    parser.add_argument("--bin", "-b", type=int, default=None)
    parser.add_argument("-majority", action="store_true")
    parser.add_argument("--firepercent", "-fp", type=float, default=0.9)
    parser.add_argument("-totalburned", action="store_true")
    args = parser.parse_args()

    if args.concat:
        concat()
    if args.graph:
        graph(args.bin)
    if args.p2g:
        p2g()
    if args.log:
        log_graph(args.bin)
    if args.majority:
        fireMajority(args.firepercent)
    if args.totalburned:
        totalBurned(args.bin)
