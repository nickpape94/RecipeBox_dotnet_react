import { RECIPE_PHOTO_UPLOAD_SUCCESS, RECIPE_PHOTO_UPLOAD_FAIL, SET_MAIN_PHOTO, PHOTO_DELETED } from './types';
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

		// return res.data;

		history.push(`/posts/${postId}`);
	} catch (err) {
		const errors = err.response.data;

		console.log(errors);
		// if (errors) {
		// 	errors.foreach((error) => dispatch(setAlert(error), 'danger'));
		// }
		dispatch({
			type: RECIPE_PHOTO_UPLOAD_FAIL
		});
	}
};
