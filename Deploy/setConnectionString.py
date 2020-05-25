import json
import os
import sys
from pathlib import Path

currentFilePath = Path(sys.argv[0])

filename = currentFilePath.parents[1] / 'Tests/Functional/functionalTesting.json'
with open(filename, 'r') as f:
    data = json.load(f)
    data['ConnectionStrings']['DefaultConnection'] = sys.argv[1]

os.remove(filename)
with open(filename, 'w') as f:
    json.dump(data, f, indent=2)
