# Plano de Migração — Remoção de `Newtonsoft.Json`

## Sumário
- 1. Resumo Executivo
- 2. Estratégia de Migração
- 3. Análise de Dependências
- 4. Plano Atômico de Remoção (All-At-Once)
- 5. Referência de Pacotes
- 6. Especificações por Projeto
- 7. Gerenciamento de Risco
- 8. Testes e Validação
- 9. Complexidade & Esforço
- 10. Estratégia de Controle de Código
- 11. Critérios de Sucesso
- 12. Checklist de Remoção

---

## 1. Resumo Executivo
- Estratégia selecionada: **All-At-Once** — remoção atômica do `Newtonsoft.Json` em todos os projetos que o referenciam.
- Escopo: três projetos (.NET Framework 4.7.2): `QuattoAPIClient.Source`, `QuattoAPIClient.ConnectionManager`, `QuattoAPIClient.UI`.
- Racional: análise mostrou ausência de usos detectáveis das APIs de `Newtonsoft.Json`. `System.Text.Json` já está presente.

## 2. Estratégia de Migração
- Abordagem: executar uma operação atômica que remova o `PackageReference` e valide build e runtime.
- Ordem (único passe atômico): pré-requisitos ? remoção nos .csproj ? restore ? build ? corrigir erros de compilação (se houver) ? validação de testes e runtime.

## 3. Análise de Dependências
- `QuattoAPIClient.Source`: `System.Data.SqlClient` 4.8.6, `System.Text.Json` 10.0.2.
- `QuattoAPIClient.ConnectionManager`: `System.Text.Json` 10.0.2, `Newtonsoft.Json` 13.0.3 (candidato à remoção).
- `QuattoAPIClient.UI`: sem `PackageReference`.
- Referências de assemblies SSIS (com HintPath) presentes — confirmar disponibilidade em agentes de build.

## 4. Plano Atômico de Remoção (All-At-Once)
TASK-000: Pré-requisitos
- Verificar SDK (.NET, MSBuild) e estado do repositório; criar branch de feature.

TASK-001: Remoção Atômica
- Atualizar `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`: remover `<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />`.
- Executar `dotnet restore`.
- Recompilar a solução (`msbuild /t:Rebuild` ou Rebuild no VS).
- Corrigir erros de compilação causados pela remoção (se houver) como parte do mesmo passe.

TASK-002: Validação de Testes e Runtime
- Executar testes automatizados; validar fluxos manuais (OAuth, chamadas HTTP, serialização, mapeamento de schema) e carregamento do componente em ambiente SSIS.

## 5. Referência de Pacotes
- Remover: `Newtonsoft.Json` 13.0.3 (apenas em `ConnectionManager`).
- Manter/confirmar: `System.Text.Json` 10.0.2.

## 6. Especificações por Projeto
- `QuattoAPIClient.ConnectionManager` (src/02_ConnectionManager)
  - Ação: remover a linha do `PackageReference` e validar build.
  - Complexidade: Baixa.

- `QuattoAPIClient.Source` (src/01_Source)
  - Ação: nenhuma necessária; validar build após remoção.

- `QuattoAPIClient.UI` (src/03_UI)
  - Ação: validar build após remoção.

## 7. Gerenciamento de Risco
- Risco primário: uso em arquivos gerados ou artefatos não escaneados.
  - Mitigação: executar busca local completa antes da remoção; manter branch/commit reversível.
- Risco secundário: dependência transitiva de terceiros.
  - Mitigação: restaurar e build para verificar resolução de versão.
- Contingência: reverter commit que removeu o pacote caso sejam detectados problemas.

## 8. Testes e Validação
- Checkpoints automatizados: `dotnet restore` ? `msbuild /t:Rebuild` sem erros.
- Checkpoints manuais: validar aquisição de token OAuth2, rotas HTTP, serialização/deserialização, mapeamento de schema, carga do componente no SSIS.

## 9. Complexidade & Esforço
- Remoção: Complexidade — Baixa. Sem estimativas de tempo.

## 10. Estratégia de Controle de Código
- Criar branch: `upgrade/remove-newtonsoftjson`.
- Commit atômico: remoção do `PackageReference` e eventuais correções de build.
- Abrir PR com checklist e logs do build.

## 11. Critérios de Sucesso
- Todos os projetos compilam sem referências a `Newtonsoft.Json`.
- Nenhuma exceção de carregamento de assembly relacionada a `Newtonsoft.Json` em runtime.
- Testes e fluxos manuais críticos passam.

## 12. Checklist Operacional
- [ ] Executar busca final por tokens `Newtonsoft.Json`, `JsonConvert`, `JObject`, `JToken`.
- [ ] Criar branch `upgrade/remove-newtonsoftjson`.
- [ ] Aplicar remoção no(s) `.csproj`.
- [ ] `dotnet restore`.
- [ ] `msbuild /t:Rebuild` (ou Rebuild no VS).
- [ ] Executar testes automatizados / testes manuais.
- [ ] Abrir PR e mesclar quando aprovado.

---

### Patch sugerido (trecho a alterar)
Substituir no arquivo `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`:

```
<ItemGroup>
    <PackageReference Include="System.Text.Json" Version="10.0.2" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

por:

```
<ItemGroup>
    <PackageReference Include="System.Text.Json" Version="10.0.2" />
</ItemGroup>
```

---

Se desejar, aplico um patch que crie esse arquivo `plan.md` no repositório ou gero um script PowerShell que execute a checklist automaticamente.
