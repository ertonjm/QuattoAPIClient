# 🚀 CI/CD Pipeline Guide

> Documentação completa de CI/CD com GitHub Actions e Azure DevOps

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [GitHub Actions](#github-actions)
3. [Azure DevOps](#azure-devops)
4. [Setup](#setup)
5. [Troubleshooting](#troubleshooting)

---

## 🎯 Visão Geral

O projeto utiliza **2 opções de CI/CD**:

### ✅ GitHub Actions (Recomendado)
- Integrado nativamente no GitHub
- Grátis para repositórios públicos
- Fácil de configurar
- Workflows: build-test.yml, deploy.yml

### ✅ Azure DevOps (Alternativo)
- Integração com Azure Services
- Melhor para enterprise
- Suporte profissional
- Pipeline: azure-pipelines.yml

---

## 📊 GitHub Actions

### Workflows Implementados

#### 1. build-test.yml (Automático)

**Trigger:**
- `push` em main/develop
- `pull_request` em main/develop
- Manual: `workflow_dispatch`

**Jobs:**
1. **build-and-test**
   - Setup .NET Framework
   - Restore NuGet packages
   - Build (Release)
   - Run 47 unit tests
   - Publish coverage
   - Upload artifacts

2. **code-quality**
   - SonarCloud scan
   - Detecta vulnerabilidades
   - Relatório de cobertura

3. **security-scan**
   - Trivy vulnerability scanner
   - Varredura de segurança
   - Upload para GitHub Security

**Tempo Estimado:** ~5-10 minutos

#### 2. deploy.yml (Manual)

**Trigger:**
- `release` criada
- Manual: `workflow_dispatch` (staging/prod)

**Jobs:**
1. **build**
   - Extrai versão do git tag
   - Build Release
   - Run tests
   - Create package

2. **deploy-staging**
   - Automático em `develop`
   - Deployment para staging
   - Smoke tests

3. **deploy-production**
   - Requer aprovação manual
   - Deploy para production
   - Verificação pós-deploy
   - Notificações

**Tempo Estimado:** ~10-15 minutos

---

## 🔧 Azure DevOps

### Pipeline: azure-pipelines.yml

**Stages:**
```
Stage 1: Build         (Compilar + Testes)
   ↓
Stage 2: QualityGate  (SonarQube)
   ↓
Stage 3: Package      (Criar artefatos)
   ↓
Stage 4: Staging      (Deploy automático em develop)
   ↓
Stage 5: Production   (Deploy com aprovação em main)
```

**Triggers:**
- Push em main, develop, feature/*
- Pull requests em main, develop

---

## 🛠️ Setup

### GitHub Actions Setup

#### Passo 1: Habilitar GitHub Actions

```
1. Vá para Settings → Actions → General
2. Selecione "Allow all actions and reusable workflows"
3. Clique em Save
```

#### Passo 2: Configurar Secrets (Opcional)

```
Settings → Secrets and variables → Actions → New repository secret
```

**Secrets para adicionar:**
```
SONAR_TOKEN         # Para SonarCloud (opcional)
CODECOV_TOKEN       # Para Codecov (opcional)
SSIS_SERVER         # Staging server address
SSIS_PROD_SERVER    # Production server address
```

#### Passo 3: Ativar Workflows

Os workflows são ativados automaticamente:
- ✅ `.github/workflows/build-test.yml` → automático em push/PR
- ✅ `.github/workflows/deploy.yml` → automático em release

#### Passo 4: Testar

```
1. Faça push de uma branch
2. Vá a Actions
3. Veja o workflow rodando
4. Espere ~5 minutos para conclusão
```

---

### Azure DevOps Setup

#### Passo 1: Conectar Repositório

```
1. Crie novo projeto em dev.azure.com
2. Pipelines → New pipeline
3. Selecione GitHub (ou repositório)
4. Autorize o acesso
5. Selecione o repositório
```

#### Passo 2: Selecionar Pipeline Config

```
1. Clique em "Existing Azure Pipelines YAML file"
2. Selecione `azure-pipelines.yml`
3. Clique Save and Run
```

#### Passo 3: Configurar Variáveis

```
Pipelines → Edit → Variables
```

**Variáveis para adicionar:**
```
sonarHostUrl        = https://sonarcloud.io
sonarToken          = seu-sonar-token
stagingServerPath   = \\staging-server\SSIS\Components
prodServerPath      = C:\Program Files\Microsoft SQL Server\160\DTS\Binn
```

#### Passo 4: Configurar Environments

```
Pipelines → Environments
├── Staging   (Sem aprovação)
└── Production (Com aprovação)
```

#### Passo 5: Ativar Pipeline

O pipeline é executado automaticamente em:
- ✅ Push em main/develop/feature/*
- ✅ Pull request em main/develop

---

## 📈 Monitorando Pipelines

### GitHub Actions

```
1. Vá a Actions
2. Veja todos os workflows
3. Clique em um run para detalhes
4. Veja logs em tempo real
```

**Status Badges:**

Adicione ao README.md:

```markdown
![Build & Test](https://github.com/user/repo/actions/workflows/build-test.yml/badge.svg)
![Deploy](https://github.com/user/repo/actions/workflows/deploy.yml/badge.svg)
```

### Azure DevOps

```
1. Vá a Pipelines
2. Clique em pipeline específico
3. Veja runs recentes
4. Clique em run para detalhes
```

---

## 🚀 Executar Manualmente

### GitHub Actions

#### Build & Test Manual

```
1. Actions tab
2. "Build & Test" workflow
3. "Run workflow" button
4. Selecione branch
5. Clique "Run workflow"
```

#### Deploy Manual

```
1. Actions tab
2. "Deploy to Production" workflow
3. "Run workflow" button
4. Selecione environment (staging/prod)
5. Clique "Run workflow"
```

### Azure DevOps

#### Queue Manual Build

```
1. Pipelines
2. Selecione pipeline
3. "Run pipeline" button
4. Selecione branch
5. Clique "Run"
```

---

## 📋 Checklist de Deploy

### Pré-Deploy

- [ ] Todos os testes passando
- [ ] Code coverage > 70%
- [ ] Sem warnings críticos
- [ ] SonarQube score OK
- [ ] Documentação atualizada
- [ ] Release notes preparadas
- [ ] Rollback plan pronto

### Deploy

- [ ] Branch correta (main para prod)
- [ ] Tag de release criada
- [ ] Backup da versão anterior
- [ ] Ambiente alvo confirmado
- [ ] Aprovação obtida (se necessário)

### Pós-Deploy

- [ ] DLLs verificados
- [ ] Testes de smoke OK
- [ ] Logs verificados
- [ ] Notificações enviadas
- [ ] Documentação atualizada

---

## 🐛 Troubleshooting

### Erro: "Build Failed - Could not locate assembly"

**Causa:** NuGet packages não restaurados  
**Solução:**

```powershell
# Limpar cache NuGet
Remove-Item $env:USERPROFILE\.nuget\packages -Recurse -Force

# Restaurar packages
nuget restore src/QuattoAPIClient.sln
```

---

### Erro: "Test Failed - XUnit not found"

**Causa:** Test project não compila  
**Solução:**

```powershell
# Rebuild test project
cd src/04_Tests
dotnet clean
dotnet restore
dotnet build
dotnet test
```

---

### Erro: "Deploy Failed - Service not running"

**Causa:** SSIS Service não iniciado  
**Solução:**

```powershell
# Verificar status
Get-Service "MsDtsServer"

# Iniciar serviço
Start-Service "MsDtsServer"

# Esperar inicializar
Start-Sleep -Seconds 10
```

---

### Erro: "Permission Denied - Cannot copy DLLs"

**Causa:** Sem permissões para copiar arquivos  
**Solução:**

```powershell
# Executar como Admin
# Ou ajustar permissões:
icacls "C:\Program Files\Microsoft SQL Server\160\DTS\Binn" /grant:r "Everyone:(OI)(CI)F"
```

---

## 📊 Badges de Status

Adicione ao README:

```markdown
# Quatto API Client for SSIS

![Build & Test](https://github.com/org/repo/actions/workflows/build-test.yml/badge.svg?branch=main)
![Deploy](https://github.com/org/repo/actions/workflows/deploy.yml/badge.svg)
[![codecov](https://codecov.io/gh/org/repo/branch/main/graph/badge.svg)](https://codecov.io/gh/org/repo)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=org_repo&metric=alert_status)](https://sonarcloud.io/dashboard?id=org_repo)
```

---

## 📈 Métricas CI/CD

```
Build Time:           ~5-10 minutos
Test Execution:       ~2-3 minutos
Code Coverage:        70%+
SonarQube Grade:      A (ideal)
Deploy Time:          ~5-10 minutos
Rollback Time:        <5 minutos
```

---

## 🔒 Segurança em CI/CD

✅ **Secrets Management**
- Use GitHub Secrets
- Nunca commitar senhas
- Rotate tokens regularmente

✅ **Approval Gates**
- Requer aprovação para prod
- Apenas maintainers aprovam
- Audit trail completo

✅ **Testing**
- 47 testes unitários
- Security scanning
- Vulnerability scanning

✅ **Artifacts**
- Assinados digitalmente
- Versionados
- Auditáveis

---

## 📞 Próximas Melhorias

- [ ] Performance tests
- [ ] Load tests
- [ ] Integration tests
- [ ] E2E tests
- [ ] Canary deployments
- [ ] Blue-green deployments
- [ ] Slack notifications
- [ ] Email notifications
- [ ] Jira integration
- [ ] Datadog monitoring

---

## 📚 Referências

- [GitHub Actions Docs](https://docs.github.com/actions)
- [Azure Pipelines Docs](https://docs.microsoft.com/azure/devops/pipelines)
- [SonarCloud](https://sonarcloud.io)
- [Codecov](https://codecov.io)

---

**Versão:** 1.0.0  
**Última Atualização:** 2026-02-20  
**Status:** ✅ Pronto para Produção

