# Merkle Tree Generator

A tool for building and verifying Merkle trees (hash trees) - cryptographic data structures used in blockchain, distributed systems, and file integrity verification.

## Usage

```bash
# Run demo
dotnet run --project MerkleTreeGenerator.csproj -- demo

# Interactive mode
dotnet run --project MerkleTreeGenerator.csproj -- interactive
```

## Examples

### Demo Output
```
=== Merkle Tree Generator Demo ===

1. Building Merkle Tree from Transactions
-----------------------------------------
Transactions: 4
Merkle Root: a3f2b8c9d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1

Tree Structure:
Level 0:
  [0] tx1_hash...
  [1] tx2_hash...
  [2] tx3_hash...
  [3] tx4_hash...
Root: a3f2b8c9...e9f0a1

2. Merkle Proof Generation & Verification
------------------------------------------
Generating proof for: TX2: Bob -> Charlie: 5 BTC
Proof hashes: 2
Proof valid: True

3. Tamper Detection
-------------------
Original: TX2: Bob -> Charlie: 5 BTC
Tampered: TX2: Bob -> Charlie: 999 BTC
Tampered proof valid: False (should be False)

4. File Integrity Verification
-------------------------------
Files: 4
File Tree Root: b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9...
File 'file2.txt' integrity: VERIFIED

5. Git-like Commit Chain
------------------------
Commits: 8
Commit Tree Root: c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0...
```

### Interactive Mode
```
mt> add Transaction1:Alice->Bob:10BTC
Added item. Total items: 1
mt> add Transaction2:Bob->Charlie:5BTC
Added item. Total items: 2
mt> add Transaction3:Charlie->Dave:2BTC
Added item. Total items: 3
mt> build
Tree built with 3 leaves
Root hash: 8f3a2b1c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a2
mt> verify Transaction1:Alice->Bob:10BTC
Item is VALID (included in tree)
mt> list
  1. Transaction1:Alice->Bob:10BTC
  2. Transaction2:Bob->Charlie:5BTC
  3. Transaction3:Charlie->Dave:2BTC
mt> quit
```

## How Merkle Trees Work

1. **Leaf Nodes**: Hash of each data item (transaction, file, etc.)
2. **Non-Leaf Nodes**: Hash of concatenated child hashes
3. **Root**: Single hash representing the entire dataset
4. **Proof**: Path from leaf to root proving inclusion

### Key Properties

- **Efficient Verification**: O(log n) to prove membership
- **Tamper Detection**: Any change invalidates the root hash
- **Compact**: Root hash represents entire dataset
- **Scalable**: Works with any number of items

## Use Cases

- **Blockchain**: Bitcoin, Ethereum use Merkle trees for transactions
- **Git**: Commit history and file integrity
- **Distributed Systems**: Data synchronization (Cassandra, DynamoDB)
- **File Systems**: IPFS, BitTorrent verify file integrity
- **Certificate Transparency**: Verify SSL certificates
- **Database Replication**: Efficient consistency checks

## Concepts Demonstrated

- Cryptographic hashing (SHA-256)
- Binary tree construction
- Merkle proof generation
- Proof verification algorithm
- Tamper detection
- Power-of-2 padding
- Bottom-up tree building
