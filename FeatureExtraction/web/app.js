const DEFAULT_WEIGHTS = {
    CyclomaticComplexity: 0.40,
    MaxDepth: 0.25,
    TotalNodes: 0.20,
    FuncCallCount: 0.10,
    ArrAccessExprCount: 0.05,
};

const PRESETS = {
    default: { ...DEFAULT_WEIGHTS },
    size: { TotalNodes: 0.5, MaxDepth: 0.3, CyclomaticComplexity: 0.2 },
    control_flow: { CyclomaticComplexity: 0.6, MaxDepth: 0.25, LogicExprCount: 0.15 },
    calls: { FuncCallCount: 0.5, CyclomaticComplexity: 0.3, TotalNodes: 0.2 },
    balanced: { CyclomaticComplexity: 0.25, MaxDepth: 0.2, TotalNodes: 0.2, FuncCallCount: 0.2, ArrAccessExprCount: 0.15 },
};

let csvData = [];
let headers = [];
let numericFeatures = [];
let selectedFeatures = new Set();
let featureWeights = {};
let featureStats = {};
let lastResults = [];

const fileDrop = document.getElementById('fileDrop');
const fileInput = document.getElementById('fileInput');
const fileInfo = document.getElementById('fileInfo');
const fileName = document.getElementById('fileName');
const rowCount = document.getElementById('rowCount');
const featureSearch = document.getElementById('featureSearch');
const presetWeights = document.getElementById('presetWeights');
const featureList = document.getElementById('featureList');
const weightSumValue = document.getElementById('weightSumValue');
const weightSumEl = document.getElementById('weightSum');
const computeBtn = document.getElementById('computeBtn');
const resetBtn = document.getElementById('resetBtn');
const emptyState = document.getElementById('emptyState');
const loading = document.getElementById('loading');
const resultsContainer = document.getElementById('resultsContainer');
const resultsBody = document.getElementById('resultsBody');
const totalFunctions = document.getElementById('totalFunctions');
const featuresUsed = document.getElementById('featuresUsed');
const scoreRange = document.getElementById('scoreRange');
const exportCsvBtn = document.getElementById('exportCsvBtn');
const exportJsonBtn = document.getElementById('exportJsonBtn');
const errorToast = document.getElementById('errorToast');

function showError(message) {
    errorToast.textContent = message;
    errorToast.classList.add('show');
    setTimeout(() => errorToast.classList.remove('show'), 5000);
}

function parseCSV(text) {
    const lines = text.trim().split('\n');
    if (lines.length < 2) throw new Error('CSV must have header and at least one data row');

    const headerLine = lines[0];
    const headers = parseCSVLine(headerLine);

    const data = [];
    for (let i = 1; i < lines.length; i++) {
        const values = parseCSVLine(lines[i]);
        if (values.length === headers.length) {
            const row = {};
            headers.forEach((h, idx) => row[h] = values[idx]);
            data.push(row);
        }
    }
    return { headers, data };
}

function parseCSVLine(line) {
    const result = [];
    let current = '';
    let inQuotes = false;
    for (let i = 0; i < line.length; i++) {
        const char = line[i];
        if (char === '"') {
            if (inQuotes && line[i + 1] === '"') {
                current += '"';
                i++;
            } else {
                inQuotes = !inQuotes;
            }
        } else if (char === ',' && !inQuotes) {
            result.push(current);
            current = '';
        } else {
            current += char;
        }
    }
    result.push(current);
    return result;
}

function detectNumericFeatures(data, headers) {
    const numeric = [];
    const sampleSize = Math.min(data.length, 100);
    for (const header of headers) {
        if (header === 'FunctionName' || header === 'SourceFile' || header === 'Language' ||
            header === 'ReturnType' || header.startsWith('ParamType') || header === 'IsRecursive') {
            continue;
        }
        let allNumeric = true;
        for (let i = 0; i < sampleSize; i++) {
            const val = data[i][header];
            if (val === '' || isNaN(Number(val))) {
                allNumeric = false;
                break;
            }
        }
        if (allNumeric) numeric.push(header);
    }
    return numeric;
}

function computeFeatureStats(data, features) {
    const stats = {};
    for (const feature of features) {
        const values = data.map(row => Number(row[feature]));
        const mean = values.reduce((a, b) => a + b, 0) / values.length;
        const variance = values.reduce((sum, v) => sum + (v - mean) ** 2, 0) / values.length;
        const stddev = Math.sqrt(variance);
        const min = Math.min(...values);
        const max = Math.max(...values);
        stats[feature] = { mean, stddev, min, max, values };
    }
    return stats;
}

function computeScores(data, weights, stats) {
    const results = data.map(row => {
        let score = 0;
        for (const [feature, weight] of Object.entries(weights)) {
            const { mean, stddev } = stats[feature];
            const value = Number(row[feature]);
            const z = stddev > 0 ? (value - mean) / stddev : 0;
            score += weight * z;
        }
        return {
            ...row,
            Score: score,
            CyclomaticComplexity: Number(row.CyclomaticComplexity) || 0,
        };
    });

    results.sort((a, b) => {
        if (b.Score !== a.Score) return b.Score - a.Score;
        if (a.SourceFile !== b.SourceFile) return a.SourceFile.localeCompare(b.SourceFile);
        return a.FunctionName.localeCompare(b.FunctionName);
    });

    return results.map((r, i) => ({ ...r, Rank: i + 1 }));
}

function renderFeatureList() {
    const search = featureSearch.value.toLowerCase();
    featureList.innerHTML = '';

    const filtered = numericFeatures.filter(f => f.toLowerCase().includes(search));
    if (filtered.length === 0) {
        featureList.innerHTML = '<p style="color:#999;font-size:0.85rem;padding:8px;">No features match</p>';
        return;
    }

    // Sort: selected features first, then alphabetically
    filtered.sort((a, b) => {
        const aSelected = selectedFeatures.has(a);
        const bSelected = selectedFeatures.has(b);
        if (aSelected !== bSelected) return aSelected ? -1 : 1;
        return a.localeCompare(b);
    });

    for (const feature of filtered) {
        const isDefault = feature in DEFAULT_WEIGHTS;
        const stats = featureStats[feature];
        const div = document.createElement('div');
        div.className = `feature-item ${selectedFeatures.has(feature) ? 'selected' : ''}`;
        div.innerHTML = `
            <input type="checkbox" class="feature-checkbox" data-feature="${feature}" ${selectedFeatures.has(feature) ? 'checked' : ''}>
            <div class="feature-info">
                <div class="feature-name">
                    ${feature}
                    ${isDefault ? '<span class="default-features-badge">default</span>' : ''}
                </div>
                <div class="feature-stats">μ=${stats.mean.toFixed(2)} σ=${stats.stddev.toFixed(2)} [${stats.min}–${stats.max}]</div>
            </div>
            <input type="number" class="weight-input" data-feature="${feature}" value="${featureWeights[feature]?.toFixed(2) || ''}" step="0.01" min="0" ${!selectedFeatures.has(feature) ? 'disabled' : ''} placeholder="0.00">
        `;
        featureList.appendChild(div);
    }

    featureList.querySelectorAll('.feature-checkbox').forEach(cb => {
        cb.addEventListener('change', e => toggleFeature(e.target.dataset.feature, e.target.checked));
    });
    featureList.querySelectorAll('.weight-input').forEach(input => {
        input.addEventListener('input', e => updateWeight(e.target.dataset.feature, parseFloat(e.target.value) || 0));
        input.addEventListener('blur', e => {
            const val = parseFloat(e.target.value) || 0;
            e.target.value = val.toFixed(2);
            updateWeight(e.target.dataset.feature, val);
        });
    });
}

function toggleFeature(feature, checked) {
    if (checked) {
        selectedFeatures.add(feature);
        if (!(feature in featureWeights)) featureWeights[feature] = 0;
    } else {
        selectedFeatures.delete(feature);
        delete featureWeights[feature];
    }
    renderFeatureList();
    updateWeightSum();
    updateComputeButton();
}

function updateWeight(feature, weight) {
    if (weight < 0) weight = 0;
    if (selectedFeatures.has(feature)) {
        featureWeights[feature] = weight;
        renderFeatureList();
        updateWeightSum();
        updateComputeButton();
    }
}

function updateWeightSum() {
    const sum = Object.values(featureWeights).reduce((a, b) => a + b, 0);
    weightSumValue.textContent = sum.toFixed(2);
    weightSumEl.classList.remove('valid', 'invalid');
    if (sum > 0) {
        weightSumEl.classList.add(Math.abs(sum - 1) < 0.01 ? 'valid' : 'invalid');
    }
}

function updateComputeButton() {
    const hasSelection = selectedFeatures.size > 0;
    const sum = Object.values(featureWeights).reduce((a, b) => a + b, 0);
    computeBtn.disabled = !hasSelection || sum === 0;
}

function applyPreset(presetName) {
    const preset = PRESETS[presetName];
    if (!preset) return;

    selectedFeatures.clear();
    featureWeights = {};

    for (const [feature, weight] of Object.entries(preset)) {
        if (numericFeatures.includes(feature)) {
            selectedFeatures.add(feature);
            featureWeights[feature] = weight;
        }
    }

    document.querySelectorAll('.preset-btn').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.preset === presetName);
    });

    renderFeatureList();
    updateWeightSum();
    updateComputeButton();
}

function renderPresets() {
    presetWeights.innerHTML = '';
    for (const name of Object.keys(PRESETS)) {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'preset-btn' + (name === 'default' ? ' active' : '');
        btn.dataset.preset = name;
        btn.textContent = name.charAt(0).toUpperCase() + name.slice(1).replace('_', ' ');
        btn.addEventListener('click', () => applyPreset(name));
        presetWeights.appendChild(btn);
    }
}

function renderResults(results) {
    resultsBody.innerHTML = '';
    const features = Array.from(selectedFeatures).sort();
    for (const row of results) {
        const scoreClass = row.Score > 1 ? 'high' : row.Score > 0 ? 'medium' : 'low';
        
        // Main row with expand button
        const tr = document.createElement('tr');
        tr.className = 'result-row';
        tr.dataset.rank = row.Rank;
        tr.innerHTML = `
            <td><button class="expand-btn" aria-label="Expand" title="Show feature values">▶</button></td>
            <td class="rank">${row.Rank}</td>
            <td class="name">${escapeHtml(row.FunctionName)}</td>
            <td class="name">${escapeHtml(row.SourceFile)}</td>
            <td class="score ${scoreClass}">${row.Score.toFixed(3)}</td>
        `;
        resultsBody.appendChild(tr);

        // Detail row (hidden by default)
        const detailTr = document.createElement('tr');
        detailTr.className = 'detail-row';
        detailTr.style.display = 'none';
        const featureCells = features.map(f => `<td class="feature-value">${Number(row[f]).toFixed(2)}</td>`).join('');
        detailTr.innerHTML = `
            <td colspan="5">
                <div class="detail-content">
                    <table class="detail-table">
                        <thead>
                            <tr>
                                <th>Feature</th>
                                <th>Value</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${features.map(f => `
                                <tr>
                                    <td class="feature-name">${escapeHtml(f)}</td>
                                    <td class="feature-value">${Number(row[f]).toFixed(2)}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                </div>
            </td>
        `;
        resultsBody.appendChild(detailTr);
    }

    // Add click handlers for expand buttons
    resultsBody.querySelectorAll('.expand-btn').forEach(btn => {
        btn.addEventListener('click', e => {
            e.stopPropagation();
            const row = btn.closest('.result-row');
            const detailRow = row.nextElementSibling;
            const isExpanded = detailRow.style.display !== 'none';
            detailRow.style.display = isExpanded ? 'none' : 'table-row';
            btn.textContent = isExpanded ? '▶' : '▼';
            btn.setAttribute('aria-label', isExpanded ? 'Expand' : 'Collapse');
            row.classList.toggle('expanded', !isExpanded);
        });
    });

    lastResults = results;

    const scores = results.map(r => r.Score);
    totalFunctions.textContent = results.length;
    featuresUsed.textContent = selectedFeatures.size;
    scoreRange.textContent = `${Math.min(...scores).toFixed(3)} to ${Math.max(...scores).toFixed(3)}`;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function exportCSV(results) {
    const features = Array.from(selectedFeatures);
    const headers = ['Rank', 'FunctionName', 'SourceFile', 'Score', ...features];
    const rows = [headers.join(',')];
    for (const row of results) {
        const values = [
            row.Rank,
            `"${row.FunctionName}"`,
            `"${row.SourceFile}"`,
            row.Score.toFixed(3),
            ...features.map(f => row[f])
        ];
        rows.push(values.join(','));
    }
    downloadFile(rows.join('\n'), 'complexity-scores.csv', 'text/csv');
}

function exportJSON(results) {
    const features = Array.from(selectedFeatures);
    const output = {
        weights: featureWeights,
        features,
        rowCount: results.length,
        results: results.map(r => ({
            rank: r.Rank,
            functionName: r.FunctionName,
            sourceFile: r.SourceFile,
            score: Number(r.Score.toFixed(3)),
            features: Object.fromEntries(features.map(f => [f, Number(r[f])]))
        }))
    };
    downloadFile(JSON.stringify(output, null, 2), 'complexity-scores.json', 'application/json');
}

function downloadFile(content, filename, mimeType) {
    const blob = new Blob([content], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
}

function handleFile(file) {
    const reader = new FileReader();
    reader.onload = e => {
        try {
            const { headers: h, data } = parseCSV(e.target.result);
            if (data.length === 0) throw new Error('No data rows found');

            if (!h.includes('FunctionName') || !h.includes('SourceFile')) {
                throw new Error('CSV must contain FunctionName and SourceFile columns');
            }

            headers = h;
            csvData = data;
            numericFeatures = detectNumericFeatures(data, headers);
            featureStats = computeFeatureStats(data, numericFeatures);

            selectedFeatures.clear();
            featureWeights = {};

            for (const feature of Object.keys(DEFAULT_WEIGHTS)) {
                if (numericFeatures.includes(feature)) {
                    selectedFeatures.add(feature);
                    featureWeights[feature] = DEFAULT_WEIGHTS[feature];
                }
            }

            fileName.textContent = file.name;
            rowCount.textContent = `${data.length} rows`;
            fileInfo.classList.add('show');
            fileDrop.classList.add('has-file');

            renderPresets();
            renderFeatureList();
            updateWeightSum();
            updateComputeButton();

            emptyState.style.display = 'none';
            resultsContainer.style.display = 'none';
        } catch (err) {
            showError(err.message);
        }
    };
    reader.readAsText(file);
}

fileDrop.addEventListener('click', () => fileInput.click());
fileDrop.addEventListener('dragover', e => { e.preventDefault(); fileDrop.classList.add('drag-over'); });
fileDrop.addEventListener('dragleave', () => fileDrop.classList.remove('drag-over'));
fileDrop.addEventListener('drop', e => {
    e.preventDefault();
    fileDrop.classList.remove('drag-over');
    if (e.dataTransfer.files.length) handleFile(e.dataTransfer.files[0]);
});
fileInput.addEventListener('change', e => {
    if (e.target.files.length) handleFile(e.target.files[0]);
});

featureSearch.addEventListener('input', renderFeatureList);

computeBtn.addEventListener('click', () => {
    loading.classList.add('show');
    computeBtn.disabled = true;

    setTimeout(() => {
        try {
            const weights = { ...featureWeights };
            const results = computeScores(csvData, weights, featureStats);
            renderResults(results);
            loading.classList.remove('show');
            resultsContainer.style.display = 'block';
            computeBtn.disabled = false;
        } catch (err) {
            showError(err.message);
            loading.classList.remove('show');
            computeBtn.disabled = false;
        }
    }, 50);
});

resetBtn.addEventListener('click', () => {
    csvData = [];
    headers = [];
    numericFeatures = [];
    selectedFeatures.clear();
    featureWeights = {};
    featureStats = {};

    fileInfo.classList.remove('show');
    fileDrop.classList.remove('has-file');
    fileInput.value = '';
    featureSearch.value = '';
    renderPresets();
    renderFeatureList();
    updateWeightSum();
    updateComputeButton();

    emptyState.style.display = 'block';
    resultsContainer.style.display = 'none';
    loading.classList.remove('show');
});

exportCsvBtn.addEventListener('click', () => {
    if (lastResults.length === 0) return showError('No results to export');
    exportCSV(lastResults);
});

exportJsonBtn.addEventListener('click', () => {
    if (lastResults.length === 0) return showError('No results to export');
    exportJSON(lastResults);
});

renderPresets();
renderFeatureList();
updateWeightSum();
updateComputeButton();