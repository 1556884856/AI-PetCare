<!--
  我的宠物页面
  列表展示 + 弹窗添加/编辑宠物
-->
<template>
  <div class="me-page"><div class="me-layout">
    <aside class="me-nav">
      <div class="user-card"><el-avatar :size="56" icon="UserFilled" /><div><h3>{{ auth.user?.nickname || auth.user?.phone }}</h3><p>{{ auth.user?.phone }}</p></div></div>
      <div class="nav-links">
        <router-link to="/me" class="nav-link">个人信息</router-link>
        <router-link to="/me/pets" class="nav-link active">我的宠物</router-link>
        <router-link to="/me/appointments" class="nav-link">我的预约</router-link>
      </div>
    </aside>
    <main class="me-content">
      <div class="content-header"><h2>🐾 我的宠物</h2><el-button type="primary" @click="showDialog = true; resetForm()">+ 添加宠物</el-button></div>
      <div class="pet-grid" v-loading="loading">
        <div class="pet-card" v-for="pet in pets" :key="pet.id">
          <div class="pet-avatar">{{ pet.type === 'Cat' ? '🐈' : '🐕' }}</div>
          <div class="pet-info"><h3>{{ pet.name }}</h3><p>{{ pet.breed }} · {{ pet.age }}个月 · {{ pet.weight }}kg</p><p v-if="pet.notes" class="pet-notes">📝 {{ pet.notes }}</p></div>
          <div class="pet-actions"><el-button size="small" @click="editPet(pet)">编辑</el-button><el-button size="small" type="danger" @click="removePet(pet.id)">删除</el-button></div>
        </div>
        <el-empty v-if="!loading && pets.length === 0" description="还没有添加宠物" />
      </div>
    </main>
  </div></div>
  <el-dialog v-model="showDialog" :title="editingId ? '编辑宠物' : '添加宠物'" width="420px">
    <el-form :model="form" label-position="top">
      <el-form-item label="名字"><el-input v-model="form.name" /></el-form-item>
      <el-form-item label="类型"><el-radio-group v-model="form.type"><el-radio value="Dog">🐕 狗狗</el-radio><el-radio value="Cat">🐈 猫咪</el-radio></el-radio-group></el-form-item>
      <el-form-item label="品种"><el-input v-model="form.breed" /></el-form-item>
      <el-form-item label="年龄（月）"><el-input-number v-model="form.age" :min="1" /></el-form-item>
      <el-form-item label="体重（kg）"><el-input-number v-model="form.weight" :min="0.5" :step="0.5" /></el-form-item>
      <el-form-item label="备注"><el-input v-model="form.notes" /></el-form-item>
    </el-form>
    <template #footer><el-button @click="showDialog = false">取消</el-button><el-button type="primary" @click="savePet">保存</el-button></template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { getMyPets, createPet, updatePet, deletePet } from '@/api/pets'
import { ElMessage, ElMessageBox } from 'element-plus'
const auth = useAuthStore()
const pets = ref<any[]>([]); const loading = ref(false); const showDialog = ref(false); const editingId = ref<number | null>(null)
const form = reactive({ name: '', type: 'Dog', breed: '', age: 12, weight: 10, notes: '' })
const resetForm = () => { editingId.value = null; Object.assign(form, { name: '', type: 'Dog', breed: '', age: 12, weight: 10, notes: '' }) }
const editPet = (pet: any) => { editingId.value = pet.id; Object.assign(form, pet); showDialog.value = true }
const fetchPets = async () => { loading.value = true; try { const res: any = await getMyPets(); pets.value = res.data || [] } catch {} finally { loading.value = false } }
const savePet = async () => { try { if (editingId.value) await updatePet(editingId.value, form); else await createPet(form); ElMessage.success('保存成功'); showDialog.value = false; fetchPets() } catch {} }
const removePet = async (id: number) => { await ElMessageBox.confirm('确定删除这只宠物吗？', '提示', { type: 'warning' }); try { await deletePet(id); ElMessage.success('删除成功'); fetchPets() } catch {} }
onMounted(fetchPets)
</script>
<style scoped>
.me-page { max-width: 1200px; margin: 0 auto; padding: 48px 24px; }
.me-layout { display: flex; gap: 32px; }
.me-nav { width: 220px; flex-shrink: 0; }
.user-card { display: flex; align-items: center; gap: 12px; padding: 20px; background: #fff; border-radius: var(--radius-md); box-shadow: var(--shadow-card); margin-bottom: 12px; }
.user-card h3 { font-size: 15px; } .user-card p { color: var(--muted); font-size: 13px; }
.nav-links { background: #fff; border-radius: var(--radius-md); box-shadow: var(--shadow-card); overflow: hidden; }
.nav-link { display: block; padding: 14px 20px; font-size: 14px; border-left: 3px solid transparent; transition: all var(--transition); }
.nav-link:hover { background: var(--primary-bg); }
.nav-link.active { border-left-color: var(--primary); color: var(--primary); background: var(--primary-bg); font-weight: 600; }
.me-content { flex: 1; background: #fff; border-radius: var(--radius-md); padding: 32px; box-shadow: var(--shadow-card); }
.content-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.content-header h2 { font-size: 22px; }
.pet-grid { display: flex; flex-direction: column; gap: 12px; }
.pet-card { display: flex; align-items: center; gap: 16px; padding: 16px 20px; background: #fafafa; border-radius: var(--radius-sm); }
.pet-avatar { font-size: 36px; }
.pet-info { flex: 1; }
.pet-info h3 { font-size: 16px; margin-bottom: 4px; }
.pet-info p { color: var(--muted); font-size: 13px; }
.pet-notes { color: var(--secondary) !important; margin-top: 4px; }
.pet-actions { display: flex; gap: 8px; }
@media (max-width: 768px) { .me-layout { flex-direction: column; } .me-nav { width: 100%; } }
</style>
