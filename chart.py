import matplotlib.pyplot as plt
file = open("C:/Users/trosf/Documents/Code/MIPS/mod-lab04-life/Data/out.txt")
lines = file.readlines()
file.close()
data_dict = {}
for i in range(len(lines)):
    lines[i] = lines[i][:len(lines[i]) - 1]

for line in lines:
    s = line.split()
    key = float(s[0].replace(',', '.'))
    value = int(s[1])
    if key not in data_dict:
        data_dict[key] =  [value]
    else:
        data_dict[key].append(value)
for key in data_dict:
    data_dict[key] = round(sum(data_dict[key])/len(data_dict[key]),2)





keys = list(data_dict.keys())
values = list(data_dict.values())

fig, ax = plt.subplots(figsize=(16,10), dpi= 80)
ax.vlines(x = keys, ymin = 0, ymax  = values, color='firebrick', alpha=0.7, linewidth=2)
ax.scatter(x = keys, y = values, s=75, color='firebrick', alpha=0.7)

# Title, Label, Ticks and Ylim
ax.set_title('Зависимость кол-ва поколений от плотности', fontdict={'size':22})
ax.set_ylabel('Кол-во поколений')
ax.set_xticks(keys)
ax.set_xticklabels(keys, rotation = 60, fontdict={'horizontalalignment': 'right', 'size':12})
ax.set_ylim(0, max(values) + 25)

# Annotate

for i in range(len(values)):
    ax.text(keys[i], values[i]+2.5, s = round(values[i], 2), horizontalalignment= 'center', verticalalignment='bottom', fontsize=14) 

plt.show()


print()