import os
import json


with open('C://Users//trosf//Documents//Code//MIPS//mod-lab04-life//settings.json', 'r+') as f:
    data = json.load(f)
    data['width'] = 25 
    data['height'] = 25
    data['liveDensity'] = 0.05
    f.seek(0)        # <--- should reset file position to the beginning.
    json.dump(data, f, indent=4)
    f.truncate()     # remove remaining part


for i in range(1, 180):
    if (i % 10 == 0):
        with open('C://Users//trosf//Documents//Code//MIPS//mod-lab04-life//settings.json', 'r+') as f:
            data = json.load(f)
            data['liveDensity']= round(data['liveDensity'] + 0.05, 2);
            f.seek(0)        # <--- should reset file position to the beginning.
            json.dump(data, f, indent=4)
            f.truncate()     # remove remaining part
    os.system('"C://Users//trosf//Documents//Code//MIPS//mod-lab04-life//Life//bin//Debug//netcoreapp8.0//Life.exe"')