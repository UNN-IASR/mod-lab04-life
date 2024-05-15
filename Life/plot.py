import json
import matplotlib.pyplot as plt

# Загрузка данных из файла
with open('density_data.json', 'r') as file:
    data = json.load(file)

# Разбор данных
densities = [item["Item1"] for item in data]
generations = [item["Item2"] for item in data]

# Построение графика
plt.figure(figsize=(10, 6))
plt.plot(densities, generations, marker='o', linestyle='-', color='b')
plt.title('Dependence of Generations to Stable State on Live Density')
plt.xlabel('Live Density')
plt.ylabel('Average Generations to Stable State')
plt.grid(True)
plt.show()
