import os
import re
import json

import matplotlib.pyplot as plt
from glob import glob


class Pattern:
    name: str
    layouts: list[list[str]]


def count_patterns(state: list[str], patterns: list[Pattern]):
    counts = []
    for pattern in patterns:
        counts.append(0)
        for layout in pattern['layouts']:
            width = len(layout[0])
            for layout_row in layout:
                if len(layout_row) > width:
                    width = len(layout_row)
            height = len(layout)
            for row_idx in range(0, len(state)):
                indexes = [m.start() for m in re.finditer(layout[0], state[row_idx])]
                for idx in indexes:
                    extra_left = 1 if 0 < idx else 0
                    extra_right = 1 if idx < len(state[row_idx]) - width else 0
                    if row_idx > len(state) - height:
                        continue
                    if row_idx > 0:
                        if state[row_idx - 1][idx - extra_left:idx + width + 1] != ''.join('0' for _i in range(0, width + extra_left + extra_right)):
                            continue
                    if idx > 0 and state[row_idx][idx - 1] != '0':
                        continue
                    if idx < len(state[row_idx]) - width and state[row_idx][idx + width] != '0':
                        continue
                    correct_height = 1
                    for _i in range(1, height):
                        if state[row_idx + _i][idx:idx + width] != layout[_i]:
                            continue
                        if idx > 0 and state[row_idx + _i][idx - 1] != '0':
                            continue
                        if idx < len(state[row_idx + _i]) - width and state[row_idx + _i][idx + width] != '0':
                            continue
                        correct_height += 1
                    if correct_height != height:
                        continue
                    if row_idx + correct_height < len(state) - 1:
                        if state[row_idx + correct_height][idx - extra_left:idx + width + 1] != ''.join('0' for _i in range(0, width + extra_left + extra_right)):
                            continue
                    counts[len(counts) - 1] += 1
    return counts


def read_patterns(folder_path: str):
    patterns = []
    suffix = '/*.json'
    file_paths: list[str] = glob(folder_path + suffix)
    for file_path in file_paths:
        with open(file_path) as f:
            pattern: Pattern = json.load(f)
            patterns.append(pattern)
    return patterns


def read_state(state_path: str):
    state = []
    with open(state_path) as f:
        for str_s in f:
            state.append('')
            for chr_c in str_s.strip().split(','):
                if chr_c != '':
                    state[len(state) - 1] += chr_c
    return state


def plot_state(state: list[str]):
    int_matrix = []
    for row in state:
        int_matrix.append([])
        for char in row:
            int_matrix[len(int_matrix) - 1].append(int(char))
    plt.imshow(int_matrix, cmap='binary', interpolation='nearest')
    plt.show()


def plot_statistics(patterns: list[Pattern], counts: list[int]):
    labels = []
    non_zero_counts = []
    for pattern_idx in range(0, len(patterns)):
        if counts[pattern_idx] != 0:
            name = patterns[pattern_idx]['name']
            labels.append(f'{name}: {counts[pattern_idx]}')
            non_zero_counts.append(counts[pattern_idx])
    if not non_zero_counts:
        print('No Patterns Found!')
    else:
        plt.pie(non_zero_counts, labels=labels)
        plt.show()


def check_vertical_symmetry(state: list[str]):
    all_pairs = 0
    all_sym_pairs = 0
    for row in state:
        pairs = len(row) // 2
        sym_pairs = 0
        for i in range(0, pairs):
            left = row[i]
            right = row[len(row)-1 - i]
            if left == right:
                sym_pairs += 1
        all_pairs += pairs
        all_sym_pairs += sym_pairs
    return all_sym_pairs / all_pairs


def check_horizontal_symmetry(state: list[str]):
    all_pairs = 0
    all_sym_pairs = 0
    for col_idx in range(0, len(state[0])):
        pairs = len(state) // 2
        sym_pairs = 0
        for i in range(0, pairs):
            upper = state[i][col_idx]
            lower = state[len(state)-1 - i][col_idx]
            if upper == lower:
                sym_pairs += 1
        all_pairs += pairs
        all_sym_pairs += sym_pairs
    return all_sym_pairs / all_pairs


def count_living_cells(state: list[str]):
    living_cells = 0
    for row in state:
        for cell in row:
            if cell == '1':
                living_cells += 1
    return living_cells


def count_dead_cells(state: list[str]):
    dead_cells = 0
    for row in state:
        for cell in row:
            if cell == '0':
                dead_cells += 1
    return dead_cells


project_root_path = os.path.dirname(os.path.abspath(__file__))
state_file_name = 'Example1.txt'
s = read_state(f'{project_root_path}/States/{state_file_name}')
# s = ['11', '01']

vs = check_vertical_symmetry(s)
hs = check_horizontal_symmetry(s)
print(f'\nVertical symmetry: {round(vs * 100, 1)}%')
print(f'Horizontal symmetry: {round(hs * 100, 1)}%')

lc = count_living_cells(s)
dc = count_dead_cells(s)
print(f'\nLiving cells: {lc}')
print(f'Dead cells: {dc}')
print(f'Total cells: {lc + dc}')

pns = read_patterns(f'{project_root_path}/Patterns')
cts = count_patterns(s, pns)
print(f'\nPatterns found: {sum(cts)}')

plot_state(s)
plot_statistics(pns, cts)
