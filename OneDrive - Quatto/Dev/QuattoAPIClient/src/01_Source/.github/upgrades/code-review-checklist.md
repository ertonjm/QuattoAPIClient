# Code Review Checklist — Remoção de Newtonsoft.Json

**PR Title**: Remove Newtonsoft.Json package (refactor to System.Text.Json)  
**Branch**: `upgrade/remove-newtonsoftjson`  
**Target**: main/master branch  
**Reviewers**: [Code Owner]  
**Date**: 2026-02-05

---

## 1. Verificações de Análise de Código

### 1.1 Mudanças de Arquivo
- [ ] **Apenas 1 arquivo modificado**: `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`
  - Verificar: `git diff` mostra somente a remoção da linha `<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />`
  - Verificar: Nenhuma outra mudança no arquivo `.csproj`

### 1.2 Nenhuma Importação Remanescente
- [ ] **Varredura por `using Newtonsoft.Json`**: ZERO ocorrências
  - Command: `git grep "using Newtonsoft.Json"`
  - Expected: Sem matches

- [ ] **Varredura por `JsonConvert`**: ZERO ocorrências
  - Command: `git grep "JsonConvert"`
  - Expected: Sem matches

- [ ] **Varredura por `JObject`**: ZERO ocorrências
  - Command: `git grep "JObject"`
  - Expected: Sem matches

- [ ] **Varredura por `JToken`**: ZERO ocorrências
  - Command: `git grep "JToken"`
  - Expected: Sem matches

- [ ] **Varredura por `Newtonsoft.Json.Linq`**: ZERO ocorrências
  - Command: `git grep "Newtonsoft.Json.Linq"`
  - Expected: Sem matches

### 1.3 Não há Comentários Remanescentes
- [ ] **Nenhum comentário referenciando Newtonsoft.Json**
  - Command: `git grep -i "newtonsoft" --include="*.cs"`
  - Expected: Sem matches (ou apenas em histórico/doc não-ativo)

### 1.4 Sem Mudanças de Lógica
- [ ] **Nenhuma mudança de lógica em código C#**
  - Verificar: Todos os arquivos `.cs` estão inalterados
  - Verificar: Somente o `.csproj` foi editado

---

## 2. Verificações de Build

### 2.1 Build Local
- [ ] **Build sem erros de compilação**
  - Command: `msbuild /t:Rebuild`
  - Expected: "Build succeeded" com 0 erros

- [ ] **Build sem warnings críticos**
  - Verificar: Nenhum warning relacionado a assembly bindings ou missing references
  - Verificar: Warnings pré-existentes (se houver) são iguais aos da main branch

- [ ] **Build time não aumentou significativamente**
  - Verificar: ~0,9s (ou similar ao baseline)

### 2.2 Restauração de Pacotes
- [ ] **`dotnet restore` executa sem erros**
  - Command: `dotnet restore`
  - Expected: "Restore succeeded" em ~0,4s

- [ ] **Nenhuma dependência transitiva de Newtonsoft.Json**
  - Command: `dotnet list package --include-transitive | grep -i newtonsoft`
  - Expected: Sem matches

### 2.3 Compatibilidade de Framework
- [ ] **Target framework mantém .NET Framework 4.7.2**
  - Verificar: `<TargetFramework>net472</TargetFramework>` presente em todos os .csproj

- [ ] **SDK .NET instalado e compatível**
  - Command: `dotnet --version`
  - Expected: Versão compatível com net472

---

## 3. Verificações de Testes

### 3.1 Testes Automatizados
- [ ] **Todos os testes passam**
  - Command: `dotnet test`
  - Expected: 100% de testes passando, 0 falhas

- [ ] **Nenhum teste novo foi adicionado**
  - Verificar: PR não adiciona testes (mudança é de remoção de dependência, não de feature)

- [ ] **Testes existentes não foram modificados**
  - Verificar: Nenhum arquivo `*.Test.cs` ou `*.Tests.cs` foi editado

### 3.2 Validação de Fluxos Críticos
- [ ] **OAuth2 Token Manager funciona com System.Text.Json**
  - Verificar: `OAuth2TokenManager.cs` usa `JsonDocument.Parse()` sem Newtonsoft.Json

- [ ] **HTTP Helper executa requisições sem erro**
  - Verificar: `HttpHelper.cs` não referencia Newtonsoft.Json
  - Verificar: Retry policy funciona corretamente

- [ ] **Schema Mapper serializa corretamente**
  - Verificar: `SchemaMapper.cs` usa `System.Text.Json`

### 3.3 Testes Manuais (se aplicável)
- [ ] **OAuth2 flow testado manualmente** (se ambiente disponível)
  - Validar: Token acquisition sem FileNotFoundException

- [ ] **HTTP GET/POST testados manualmente** (se ambiente disponível)
  - Validar: Requisições completam sem erro

- [ ] **Carregamento de componente SSIS testado** (se ambiente disponível)
  - Validar: DLL carrega sem TypeLoadException ou FileNotFoundException

---

## 4. Verificações de Dependências

### 4.1 Dependências do Projeto
- [ ] **System.Text.Json 10.0.2 presente e ativo**
  - Verificar: Pacote listado em `PackageReference` em ambos:
    - `src/01_Source/QuattoAPIClient.Source.csproj`
    - `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`

- [ ] **System.Data.SqlClient 4.8.6 mantido inalterado**
  - Verificar: Versão não foi alterada acidentalmente

- [ ] **Sem pacotes duplicados ou conflitantes**
  - Command: `dotnet list package`
  - Expected: Nenhuma referência a Newtonsoft.Json

### 4.2 Pacotes do UI Project
- [ ] **QuattoAPIClient.UI sem PackageReference**
  - Verificar: Arquivo `.csproj` do UI não contém mudanças
  - Verificar: Project references mantidas intactas

### 4.3 Lock Files
- [ ] **packages.lock.json (se presente) removeu Newtonsoft.Json**
  - Command: `git grep "Newtonsoft.Json" -- "*.lock.json"`
  - Expected: Sem matches

---

## 5. Verificações de Segurança

### 5.1 Nenhuma Introdução de Vulnerabilidades
- [ ] **Nenhum novo pacote introduzido**
  - Verificar: PR remove 1 pacote, não adiciona nenhum

- [ ] **System.Text.Json é versão segura**
  - Verificar: v10.0.2 não tem vulnerabilidades conhecidas (CVE check)

- [ ] **Nenhum código malicioso ou injetado**
  - Verificar: Diff contém apenas remoção de PackageReference
  - Verificar: Sem mudanças em código de lógica crítica

### 5.2 Conformidade com Políticas
- [ ] **Nenhuma chave/secret exposta**
  - Verificar: Diff não contém credenciais, tokens ou secrets

- [ ] **Nenhuma violação de política de dependências**
  - Verificar: Remoção de dependência está dentro de políticas corporativas

---

## 6. Verificações de Documentação

### 6.1 Artefatos de Migração
- [ ] **assessment.md presente e completo**
  - Verificar: Arquivo em `.github/upgrades/assessment.md`
  - Verificar: Contém análise de dependências e riscos

- [ ] **plan.md presente e completo**
  - Verificar: Arquivo em `.github/upgrades/plan.md`
  - Verificar: Contém estratégia e checklist operacional

- [ ] **tasks.md presente com status final**
  - Verificar: Arquivo em `.github/upgrades/tasks.md`
  - Verificar: Todas as 3 tarefas marcadas como [?] Complete

- [ ] **execution_report.md presente**
  - Verificar: Arquivo em `.github/upgrades/execution_report.md`
  - Verificar: Contém sumário executivo e métricas

### 6.2 Commit Message
- [ ] **Mensagem de commit clara e descritiva**
  - Expected: `Remove Newtonsoft.Json package (refactor to System.Text.Json)`
  - Verificar: Mensagem segue padrão de commits do projeto

### 6.3 Descrição da PR
- [ ] **PR description contém contexto**
  - Verificar: Referência a assessment.md, plan.md e execution_report.md
  - Verificar: Explica por que Newtonsoft.Json foi removido
  - Verificar: Descreve impacto (zero código dependente)

---

## 7. Verificações de Compatibilidade

### 7.1 Compatibilidade Horizontal (Projetos)
- [ ] **QuattoAPIClient.Source compila sem erro**
  - Verificar: Build individual do projeto passa

- [ ] **QuattoAPIClient.ConnectionManager compila sem erro**
  - Verificar: Build individual do projeto passa
  - Verificar: Referências SSIS (HintPath) ainda válidas

- [ ] **QuattoAPIClient.UI compila sem erro**
  - Verificar: Build individual do projeto passa
  - Verificar: Project references mantidas

### 7.2 Compatibilidade com SSIS
- [ ] **Assemblies SSIS ainda carregam**
  - Verificar: Microsoft.SqlServer.DTSPipelineWrap pode ser carregado
  - Verificar: Microsoft.SqlServer.ManagedDTS pode ser carregado

- [ ] **Componentes custom SSIS ainda funcionam**
  - Verificar: CorporateApiSource component carrega sem FileNotFoundException
  - Verificar: Connection Manager carrega sem TypeLoadException

### 7.3 Compatibilidade com CI/CD
- [ ] **Build server consegue restaurar pacotes**
  - Verificar: Nenhuma dependência em paths locais hardcoded
  - Verificar: HintPath para SSIS é relativo ou configurável

- [ ] **Testes executam em CI/CD pipeline**
  - Verificar: Nenhuma dependência em ambiente local específico

---

## 8. Verificações de Performance

### 8.1 Nenhuma Regressão
- [ ] **Build time não aumentou**
  - Expected: ~0,9s (mesmo que antes)

- [ ] **Test execution time não aumentou**
  - Expected: Testes executam no mesmo tempo

- [ ] **Runtime performance mantida ou melhorada**
  - Expected: Sem degradação de performance (System.Text.Json é mais rápido)

### 8.2 Footprint
- [ ] **Tamanho total de dependências reduzido**
  - Expected: 1 PackageReference removida = menos dependências

- [ ] **Sem regressão de tamanho de output**
  - Expected: Arquivos compilados não aumentaram em tamanho

---

## 9. Verificações de Integração

### 9.1 Merge Conflicts
- [ ] **Nenhum merge conflict**
  - Verificar: PR não tem conflitos com main/master
  - Verificar: Merge será clean (ff ou regular merge possível)

### 9.2 Branch State
- [ ] **Branch está up-to-date com main/master**
  - Verificar: `git log --oneline main..upgrade/remove-newtonsoftjson` mostra apenas commits do feature

- [ ] **Nenhum commit acidental ou não-planejado**
  - Verificar: Somente 1 commit com a mudança (ou rebase confirmado)

---

## 10. Verificações Finais

### 10.1 Revisão de Código
- [ ] **Código foi revisado por pelo menos 1 pessoa**
  - Reviewer: [Name]
  - Date: [Date]
  - Comments: [Approvals/Suggestions]

### 10.2 Aprovação de Deploy
- [ ] **Produto owner aprovou a mudança**
  - Approver: [Name]
  - Date: [Date]

### 10.3 Ready to Merge
- [ ] **Todos os checkboxes acima foram verificados** ?
- [ ] **Build pipeline passou** (se CI/CD configurado)
- [ ] **Code review aprovada** ?
- [ ] **Nenhuma issue aberta relacionada**

---

## Sumário Executivo para Revisores

### Mudança Proposta
Remover a dependência não-utilizada `Newtonsoft.Json` (versão 13.0.3) do projeto `QuattoAPIClient.ConnectionManager`. 

### Por Quê?
- Zero usos detectados de APIs Newtonsoft.Json no código-fonte
- `System.Text.Json` (já presente) é suficiente e mais performático
- Reduz superfície de potenciais vulnerabilidades

### Impacto
- ? **Nenhum impacto em funcionalidades**
- ? **Zero código quebrado**
- ? **Build mantém sucesso**
- ? **Testes continuam passando**
- ? **SSIS components funcionam normalmente**

### Risco
- ?? **Mínimo** — Mudança isolada, bem-testada, reversível

### Tempo de Revisão Estimado
- ~10-15 minutos para revisão completa

---

## Notas para Revisores

1. **Esta é uma operação atômica All-At-Once** — todas as validações foram feitas antes da PR
2. **Assessment realizado** — veja `.github/upgrades/assessment.md` para análise detalhada
3. **Plan documentado** — veja `.github/upgrades/plan.md` para estratégia de migração
4. **Execution completa** — veja `.github/upgrades/execution_report.md` para resultados
5. **Zero impacto técnico** — somente 1 linha removida de arquivo `.csproj`

---

## Instruções de Merge

### Após Aprovação
1. Faça **Squash Merge** (recomendado) ou **Regular Merge**
2. Delete branch após merge: `git push origin --delete upgrade/remove-newtonsoftjson`
3. Deploy em staging (se houver) ou direto em produção (se confiante)

### Pós-Merge Monitoring
- Monitorar logs de aplicação por 24h
- Procurar por qualquer `FileNotFoundException` ou `TypeLoadException` relacionada a Newtonsoft.Json
- Validar fluxos críticos em produção (OAuth2, HTTP, Schema Mapping)

---

**Checklist Versão**: 1.0  
**Última Atualização**: 2026-02-05  
**Gerado por**: GitHub Copilot App Modernization Agent
