# Bloom Filter

A space-efficient probabilistic data structure for testing set membership. Bloom filters can tell you if an item is definitely NOT in a set or PROBABLY in a set, with a configurable false positive rate.

## Usage

```bash
# Run demo
dotnet run --project BloomFilter.csproj -- demo

# Interactive mode
dotnet run --project BloomFilter.csproj -- interactive
```

## Examples

### Demo Output
```
=== Bloom Filter Demo ===

Bloom Filter Configuration:
  Expected elements: 100
  Target false positive rate: 1%
  Bit array size: 961 bits
  Hash functions: 7

Adding items to bloom filter:
  Added: apple
  Added: banana
  Added: cherry
  ...

Testing membership:
  apple        => Probably in set
  banana       => Probably in set
  kiwi         => Definitely NOT in set
  mango        => Definitely NOT in set

Statistics:
  Items added: 8
  Bits set: 56
  Fill ratio: 5.83%
  Estimated false positive rate: 0.0002%

Use Case: Password Blacklist Check
  password                  => REJECTED (in blacklist)
  MyStr0ng!Pass#2024        => OK (not in blacklist)
  qwerty                    => REJECTED (in blacklist)
  X#9kL@mP$vQ2              => OK (not in blacklist)
```

### Interactive Mode
```
bf> add apple
Added 'apple'
  Items: 1, Bits set: 7, Fill: 0.73%
bf> add banana
Added 'banana'
  Items: 2, Bits set: 14, Fill: 1.46%
bf> contains apple
'apple' is PROBABLY in the set
bf> contains orange
'orange' is DEFINITELY NOT in the set
bf> stats
  Items: 2, Bits set: 14, Fill: 1.46%
bf> quit
```

## How Bloom Filters Work

1. **Bit Array**: A fixed-size array of bits, initially all 0
2. **Hash Functions**: Multiple independent hash functions
3. **Add**: Hash the item with each function, set corresponding bits to 1
4. **Check**: Hash the item, if ALL corresponding bits are 1, item is probably in set

### Key Properties

- **Space Efficient**: Uses far less memory than storing actual items
- **False Positives**: May incorrectly say an item is in the set (but rate is configurable)
- **No False Negatives**: If it says an item is NOT in the set, it's definitely not
- **No Deletion**: Standard bloom filters don't support removal (would affect other items)

### Optimal Configuration

For `n` expected elements and false positive rate `p`:
- **Bit array size**: `m = -n * ln(p) / (ln(2))^2`
- **Hash functions**: `k = m/n * ln(2)`

## Use Cases

- **Password blacklists**: Quickly reject common passwords
- **Web crawlers**: Track visited URLs without storing all of them
- **Cache filtering**: Avoid cache lookups for definitely-missing items
- **Database query optimization**: Skip disk lookups for non-existent keys
- **Network security**: Block known malicious IPs/domains
- **Spell checkers**: Store dictionary words efficiently

## Concepts Demonstrated

- Probabilistic data structures
- Bit array manipulation
- Multiple hash functions (double hashing)
- MD5-based hashing with seeds
- False positive rate calculation
- Space-time tradeoffs
- Optimal parameter calculation
