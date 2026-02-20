# Sample Projects - Quatto API Client

> Exemplos práticos de uso do Quatto API Client for SSIS

---

## 📋 Sample Projects Available

### 1. SimpleApiConsumer (Básico)
**Nível:** Iniciante  
**Tempo:** 30 minutos  
**Conceitos:** Basic API integration, configuration

Integração simples com API pública (GitHub API)

### 2. AdvancedDataPipeline (Avançado)
**Nível:** Intermediário  
**Tempo:** 2 horas  
**Conceitos:** Complex pipelines, watermark, incremental loads

Pipeline de dados completo com múltiplas APIs

### 3. OAuth2Integration (OAuth2)
**Nível:** Avançado  
**Tempo:** 1-2 horas  
**Conceitos:** OAuth2 flow, token management, security

Integração segura com APIs que requerem OAuth2

### 4. RealWorldECommerce (Completo)
**Nível:** Avançado  
**Tempo:** 4-6 horas  
**Conceitos:** Multi-source integration, orchestration, monitoring

Sistema de integração de e-commerce completo

---

## 🚀 Como Usar

### Pré-requisitos
```
✅ Visual Studio 2022
✅ .NET Framework 4.7.2
✅ Quatto API Client instalado
✅ SQL Server 2022 com SSIS
```

### Structure
```
samples/
├── 01_SimpleApiConsumer/
│   ├── SSISPackage.dtsx
│   ├── README.md
│   └── Screenshots/
├── 02_AdvancedDataPipeline/
│   ├── SSISPackage.dtsx
│   ├── README.md
│   ├── Configuration/
│   └── Scripts/
├── 03_OAuth2Integration/
│   ├── SSISPackage.dtsx
│   ├── README.md
│   └── Credentials/
└── 04_RealWorldECommerce/
    ├── SSISPackage.dtsx
    ├── README.md
    ├── Database/
    ├── Configuration/
    └── Documentation/
```

---

## 📖 Quick Start

### Sample 1: Simple API Consumer

**Goal:** Fetch data from GitHub API

**Steps:**
1. Open Visual Studio
2. Open sample in `samples/01_SimpleApiConsumer/`
3. Configure connection to GitHub API
4. Run SSIS package
5. Verify data in destination

**Key Learnings:**
- Component configuration
- Connection manager setup
- Basic error handling

---

### Sample 2: Advanced Data Pipeline

**Goal:** Multi-source ETL with incremental load

**Steps:**
1. Setup sample databases
2. Configure multiple API connections
3. Setup watermark tables
4. Run SSIS package
5. Monitor execution logs

**Key Learnings:**
- Incremental data loading
- Watermark management
- Error handling
- Logging

---

### Sample 3: OAuth2 Integration

**Goal:** Secure API integration with OAuth2

**Steps:**
1. Configure OAuth2 credentials
2. Setup token refresh
3. Configure API endpoints
4. Run SSIS package
5. Verify token management

**Key Learnings:**
- OAuth2 flow
- Token management
- Security best practices
- Error handling

---

### Sample 4: Real World E-Commerce

**Goal:** Complete e-commerce data integration

**Steps:**
1. Setup sample databases
2. Configure e-commerce APIs
3. Setup data warehouse
4. Configure ETL pipelines
5. Setup monitoring
6. Run orchestration

**Key Learnings:**
- Complex orchestration
- Multiple data sources
- Error handling & retry
- Monitoring & alerting
- Performance optimization

---

## 📚 Documentation Files

Each sample includes:

| File | Purpose |
|------|---------|
| **README.md** | Setup and execution guide |
| **ARCHITECTURE.md** | Design decisions (if complex) |
| **SCREENSHOTS.md** | Visual walkthrough |
| **TROUBLESHOOTING.md** | Common issues & solutions |
| **PERFORMANCE.md** | Performance characteristics |

---

## 🎓 Learning Outcomes

### After Sample 1 (30 min)
- ✅ Understand component basics
- ✅ Configure connection manager
- ✅ Create simple SSIS package

### After Sample 2 (2 hours)
- ✅ Implement incremental loads
- ✅ Manage watermarks
- ✅ Handle data transformations

### After Sample 3 (1-2 hours)
- ✅ Implement OAuth2
- ✅ Manage tokens securely
- ✅ Handle authentication errors

### After Sample 4 (4-6 hours)
- ✅ Orchestrate complex pipelines
- ✅ Monitor executions
- ✅ Optimize performance
- ✅ Handle production scenarios

---

## 🔗 Prerequisites & Setup

### System Requirements
```
OS:              Windows 10/11 or Windows Server 2019+
SQL Server:      2022
SSIS:            v17.100
Visual Studio:   2022 18.3.1+
.NET Framework:  4.7.2+
```

### Required Accounts
- GitHub (for Sample 1)
- OAuth2 provider (for Sample 3)
- E-commerce API provider (for Sample 4)

### Configuration Files
- Connection strings
- API credentials (encrypted)
- Database connection details

---

## 📊 Sample Comparison

| Feature | Sample 1 | Sample 2 | Sample 3 | Sample 4 |
|---------|----------|----------|----------|----------|
| **Difficulty** | Beginner | Intermediate | Advanced | Advanced |
| **Time** | 30 min | 2 hours | 1-2 hours | 4-6 hours |
| **APIs** | 1 | 3+ | 1 (OAuth2) | 5+ |
| **Transformations** | None | Yes | Yes | Complex |
| **Watermark** | No | Yes | No | Yes |
| **OAuth2** | No | No | Yes | Maybe |
| **Monitoring** | Basic | Yes | Yes | Advanced |

---

## 🆘 Support

### If You Get Stuck

1. **Check README.md in sample folder**
2. **Review TROUBLESHOOTING.md**
3. **Check main documentation:**
   - [INSTALLATION.md](../INSTALLATION.md)
   - [ARCHITECTURE.md](../ARCHITECTURE.md)
   - [LOGGING_GUIDE.md](../LOGGING_GUIDE.md)

4. **Check logs:**
   - SSIS Execution logs
   - Component logs
   - Application logs

---

## 🤝 Contributing Samples

Want to contribute a sample?

1. **Create new folder:** `samples/XX_YourSample/`
2. **Include:**
   - SSIS package (.dtsx)
   - README.md with setup steps
   - Configuration examples
   - Database scripts (if needed)
3. **Document:**
   - Learning outcomes
   - Key concepts
   - Troubleshooting
4. **Submit pull request**

See [CONTRIBUTING.md](../CONTRIBUTING.md) for guidelines.

---

## 📝 Next Steps

### Beginner Path
```
Sample 1 (30 min)
  ↓
LOGGING_GUIDE.md (15 min)
  ↓
ARCHITECTURE.md (20 min)
  ↓
Sample 2 (2 hours)
```

### Intermediate Path
```
Sample 2 (2 hours)
  ↓
Sample 3 (1-2 hours)
  ↓
PERFORMANCE_GUIDE.md (20 min)
  ↓
Sample 4 (4-6 hours)
```

### Advanced Path
```
Sample 3 (1-2 hours)
  ↓
Sample 4 (4-6 hours)
  ↓
Create your own sample!
```

---

## 📚 Related Documentation

- [Main README](../MAIN_README.md)
- [Installation Guide](../INSTALLATION.md)
- [Architecture](../ARCHITECTURE.md)
- [Logging Guide](../LOGGING_GUIDE.md)
- [CI/CD Guide](../CI_CD_GUIDE.md)

---

**Start with Sample 1 and progress at your own pace!** 🚀

Last Updated: 2026-02-20  
Version: 1.0.0

