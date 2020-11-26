import {
	RECIPE_PHOTO_UPLOAD_SUCCESS,
	RECIPE_PHOTO_UPLOAD_FAIL,
	RECIPE_PHOTO_DELETED,
	RECIPE_PHOTO_DELETION_ERROR,
	USER_PHOTO_UPLOAD_SUCCESS,
	USER_PHOTO_UPLOAD_FAIL,
	USER_PHOTO_DELETED,
	USER_PHOTO_DELETION_ERROR
} from './types';
import axios from 'axios';
import { setAlert } from './alert';

// Add photo for recipe
export const addRecipePhotos = (postId, history, formData) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		// First get post, check to see if the post belongs to currently logged in user, if true proceed, else redirect to posts. (try using match.params.id and equaste to userId)

		const res = await axios.post(`/api/posts/${postId}/photos`, formData, config);

		dispatch({
			type: RECIPE_PHOTO_UPLOAD_SUCCESS,
			payload: res.data
		});

		setTimeout(() => history.push(`/posts/${postId}`), 800);
	} catch (err) {
		dispatch({
			type: RECIPE_PHOTO_UPLOAD_FAIL,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Delete recipe photo
export const deleteRecipePhoto = (postId, photoId) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		await axios.delete(`/api/posts/${postId}/photos/${photoId}`, config);

		dispatch({
			type: RECIPE_PHOTO_DELETED,
			payload: photoId
		});
	} catch (err) {
		dispatch({
			type: RECIPE_PHOTO_DELETION_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Add profile photo
export const addUserPhoto = (userId, formData) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		const res = await axios.post(`/api/users/${userId}/photos`, formData, config);

		dispatch({
			type: USER_PHOTO_UPLOAD_SUCCESS,
			payload: res.data
		});
	} catch (err) {
		console.log(err);

		dispatch({
			type: USER_PHOTO_UPLOAD_FAIL,
			payload: {
				msg: err.response.statusText,
				status: err.response.status
			}
		});
	}
};

// Delete profile photo
export const deleteUserPhoto = (userId, photoId) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		await axios.delete(`/api/users/${userId}/photos/${photoId}`, config);

		dispatch({
			type: USER_PHOTO_DELETED,
			payload: photoId
		});
	} catch (err) {
		console.log(err);

		dispatch({
			type: USER_PHOTO_DELETION_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
